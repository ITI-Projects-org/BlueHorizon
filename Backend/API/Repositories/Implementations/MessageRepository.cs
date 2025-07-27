using API.Models;
using API.Repositories.Interfaces;
using API.DTOs.MessageDTO; // تأكد من إضافة هذا الـ using
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs.MessageDTO;

namespace API.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        // تأكد من استخدام _context اللي بتورثه من GenericRepository أو تعرفه هنا لو مفيش Inherited field
        private readonly BlueHorizonDbContext _context;

        public MessageRepository(BlueHorizonDbContext context) : base(context)
        {
            _context = context; // تخزين الـ context لاستخدامه في queries مباشرة
        }

        public async Task<IEnumerable<MessageDto>> GetChatBetweenUsersAsync(string currentUserId, string otherUserId)
        {
            // جلب الرسائل وعمل Include للمرسل والمستقبل وعمل Mapping للـ MessageDto مباشرة
            var messages = await _context.Messages
                .Include(m => m.SenderUser) // تأكد أن Message Model فيه SenderUser navigation property
                .Include(m => m.ReceiverUser) // تأكد أن Message Model فيه ReceiverUser navigation property
                .Where(m =>
                    (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == currentUserId)
                )
                .OrderBy(m => m.TimeStamp)
                .Select(m => new MessageDto // عمل Projecting للـ MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderUserName = m.SenderUser.UserName, // جلب اسم المرسل
                    ReceiverId = m.ReceiverId,
                    ReceiverUserName = m.ReceiverUser.UserName, // جلب اسم المستقبل
                    MessageContent = m.MessageContent,
                    TimeStamp = m.TimeStamp,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            // بعد جلب الرسائل، قم بتعليم الرسائل المستلمة كمقروءة
            await MarkMessagesAsReadAsync(currentUserId, otherUserId);

            return messages;
        }

        public async Task<IEnumerable<InboxItemDto>> GetInboxAsync(string currentUserId)
        {
            // جلب جميع الرسائل التي يكون المستخدم طرفًا فيها
            var allMessages = await _context.Messages
                .Include(m => m.SenderUser) // Include عشان تقدر تجيب اسم المستخدم
                .Include(m => m.ReceiverUser) // Include عشان تقدر تجيب اسم المستخدم
                .Where(m => m.ReceiverId == currentUserId || m.SenderId == currentUserId)
                .ToListAsync(); // ToListAsync() هنا عشان الـ GroupBy تشتغل في الـ Memory بعد كده

            // تجميع الرسائل حسب المحادثة (الشخص الآخر)
            var conversations = allMessages
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new // Select an anonymous type to hold aggregated data
                {
                    OtherUserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.TimeStamp).FirstOrDefault(),
                    UnreadCount = g.Count(m => m.ReceiverId == currentUserId && !m.IsRead)
                })
                .OrderByDescending(x => x.LastMessage.TimeStamp) // ترتيب المحادثات بناءً على آخر رسالة
                .ToList();

            var inboxItems = new List<InboxItemDto>();

            foreach (var conv in conversations)
            {
                if (conv.LastMessage == null) continue; // تأكد إن فيه رسائل في المحادثة

                // جلب بيانات المستخدم الآخر (لو محتاجها تاني غير الـ UserName)
                var otherUser = await _context.Users.FindAsync(conv.OtherUserId);
                if (otherUser == null) continue;

                inboxItems.Add(new InboxItemDto
                {
                    OtherUserId = otherUser.Id,
                    OtherUserName = otherUser.UserName, // استخدم UserName من ApplicationUser
                    LastMessageContent = conv.LastMessage.MessageContent,
                    LastMessageTimestamp = conv.LastMessage.TimeStamp,
                    IsLastMessageFromCurrentUser = conv.LastMessage.SenderId == currentUserId,
                    UnreadMessageCount = conv.UnreadCount
                });
            }

            return inboxItems;
        }

        public async Task MarkMessagesAsReadAsync(string currentUserId, string otherUserId)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverId == currentUserId && m.SenderId == otherUserId && !m.IsRead)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}