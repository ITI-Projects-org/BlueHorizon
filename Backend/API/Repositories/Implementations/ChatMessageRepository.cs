using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
    {
        private readonly BlueHorizonDbContext _context;

        public ChatMessageRepository(BlueHorizonDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatMessage>> GetUserChatHistoryAsync(string userId, int page = 1, int pageSize = 50)
        {
            return await _context.ChatMessages
                .Where(cm => cm.UserId == userId)
                .OrderByDescending(cm => cm.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(cm => cm.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ClearUserChatHistoryAsync(string userId)
        {
            try
            {
                var userMessages = await _context.ChatMessages
                    .Where(cm => cm.UserId == userId)
                    .ToListAsync();

                _context.ChatMessages.RemoveRange(userMessages);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
