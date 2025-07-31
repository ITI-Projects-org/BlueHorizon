namespace API.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        T UpdateByIdAsync(int id, T entity);
        T UpdateByIdAsync(string id, T entity);
        Task DeleteByIdAsync(int id);
        Task DeleteByIdAsync(string id);


    }
}
