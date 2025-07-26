using API.Models;
using API.Repositories.Interfaces;
using API.DTOs.MessageDTO;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }

        public async Task<IEnumerable<MessageDto>> GetChatBetweenUsersAsync(string currentUserId, string otherUserId)
        {
            var messages = await _context.Messages
                .Include(m => m.SenderUser)
                .Include(m => m.ReceiverUser)
                .Where(m =>
                    (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == currentUserId)
                )
                .OrderBy(m => m.TimeStamp)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderUserName = m.SenderUser.UserName,
                    ReceiverId = m.ReceiverId,
                    ReceiverUserName = m.ReceiverUser.UserName,
                    MessageContent = m.MessageContent,
                    TimeStamp = m.TimeStamp,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            await MarkMessagesAsReadAsync(currentUserId, otherUserId);

            return messages;
        }

        public async Task<IEnumerable<InboxItemDto>> GetInboxAsync(string currentUserId)
        {
            var allMessages = await _context.Messages
                .Include(m => m.SenderUser)
                .Include(m => m.ReceiverUser)
                .Where(m => m.ReceiverId == currentUserId || m.SenderId == currentUserId)
                .ToListAsync();

            var conversations = allMessages
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new // Select an anonymous type to hold aggregated data
                {
                    OtherUserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.TimeStamp).FirstOrDefault(),
                    UnreadCount = g.Count(m => m.ReceiverId == currentUserId && !m.IsRead)
                })
                .OrderByDescending(x => x.LastMessage.TimeStamp)
                .ToList();

            var inboxItems = new List<InboxItemDto>();

            foreach (var conv in conversations)
            {
                if (conv.LastMessage == null) continue;

                var otherUser = await _context.Users.FindAsync(conv.OtherUserId);
                if (otherUser == null) continue;

                inboxItems.Add(new InboxItemDto
                {
                    OtherUserId = otherUser.Id,
                    OtherUserName = otherUser.UserName,
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