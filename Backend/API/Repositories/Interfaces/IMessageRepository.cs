using API.DTOs.MessageDTO;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<InboxItemDto>> GetInboxAsync(string userId);
        Task MarkMessagesAsReadAsync(string currentUserId, string otherUserId);
        Task<IEnumerable<MessageDto>> GetChatBetweenUsersAsync(string currentUserId, string otherUserId);
    }


}
