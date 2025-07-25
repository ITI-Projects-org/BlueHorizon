using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class PaymentTransactionRepository  : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }
    }
    
}
