namespace Village_System.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id);
        T UpdateByIdAsync(int id, T entity);
        T UpdateByIdAsync(string id, T entity);
        void DeleteByIdAsync(int id);
        void DeleteByIdAsync(string id);

    }
}
