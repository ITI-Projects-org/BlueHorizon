using API.DTOs.MessageDTO;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {

        // تعديل الـ Signature بتاع الـ GetInboxAsync
        Task<IEnumerable<InboxItemDto>> GetInboxAsync(string userId);

        // ميثود جديدة لتعليم الرسائل كمقروءة
        Task MarkMessagesAsReadAsync(string currentUserId, string otherUserId);

        // تعديل الـ Signature بتاع الـ GetChatBetweenUsersAsync عشان يرجع MessageDto
        Task<IEnumerable<MessageDto>> GetChatBetweenUsersAsync(string currentUserId, string otherUserId);
    }


}
