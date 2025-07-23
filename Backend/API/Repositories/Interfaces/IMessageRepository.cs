using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        // داخل IMessageRepository
        Task<List<Message>> GetChatBetweenUsersAsync(string currentUserId, string otherUserId);
        Task<List<Message>> GetInboxAsync(string currentUserId);

    }
}
