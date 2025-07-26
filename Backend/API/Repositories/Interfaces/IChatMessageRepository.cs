using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IChatMessageRepository : IGenericRepository<ChatMessage>
    {
        Task<IEnumerable<ChatMessage>> GetUserChatHistoryAsync(string userId, int page = 1, int pageSize = 50);
        Task<bool> ClearUserChatHistoryAsync(string userId);
    }
}
