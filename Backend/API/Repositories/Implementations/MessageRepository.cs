using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(VillageSystemDbContext _context) : base(_context)
        {
        }
    }
}
