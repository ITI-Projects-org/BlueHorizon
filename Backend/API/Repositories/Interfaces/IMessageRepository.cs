using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>> GetChatBetweenUsersAsync(string currentUserId, string otherUserId);
        Task<List<Message>> GetInboxAsync(string currentUserId);

    }
}
