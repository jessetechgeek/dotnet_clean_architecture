namespace HR.LeaveManagement.Application.Contracts.Persistence;

public interface IGenericRepository<T> where T : class
{
    //Task<IReadOnlyList<T>> GetAllAsync();

    Task<List<T>> GetAllAsync();

    Task<T> GetByIdAsync(int id);

    Task<T> AddAsync(T entity);

    Task<T> UpdateAsync(T entity);

    Task<T> DeleteAsync(T entity);
}
