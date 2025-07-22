using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }
    }
}
