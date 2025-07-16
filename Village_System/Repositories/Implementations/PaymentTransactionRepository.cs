using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class PaymentTransactionRepository  : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(VillageSystemDbContext _context) : base(_context)
        {
        }
    }
    
}
