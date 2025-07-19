using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class PaymentTransactionRepository  : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(VillageSystemDbContext _context) : base(_context)
        {
        }
    }
    
}
