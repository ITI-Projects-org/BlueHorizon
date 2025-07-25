    using API.Models;
    using API.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    namespace API.Repositories.Implementations
    {
        public class MessageRepository : GenericRepository<Message>, IMessageRepository
        {
            public MessageRepository(BlueHorizonDbContext _context) : base(_context)
            {

            }
            public async Task<List<Message>> GetChatBetweenUsersAsync(string currentUserId, string otherUserId)
            {
                return await _context.Messages
                    .Where(m =>
                        (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                        (m.SenderId == otherUserId && m.ReceiverId == currentUserId)
                    )
                    .OrderBy(m => m.TimeStamp)
                    .ToListAsync();
            }

        public async Task<List<Message>> GetInboxAsync(string currentUserId)
        {
            var allMessages = await _context.Messages
                .Where(m => m.ReceiverId == currentUserId || m.SenderId == currentUserId)
                .ToListAsync();

            var latestMessages = allMessages
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.TimeStamp).FirstOrDefault())
                .OrderByDescending(m => m.TimeStamp)
                .ToList();

            return latestMessages;
        }

    }
}
