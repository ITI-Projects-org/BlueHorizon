using API.Models;

namespace API.Repositories.Interfaces
{
    public interface ITenantRepository : IGenericRepository<Tenant> { 
       
        Task<string> GetTenantNameBuUserId(string userId);
    }
}
