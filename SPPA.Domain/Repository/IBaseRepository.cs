using System.Linq.Expressions;

namespace SPPA.Domain.Repository;

public interface IBaseRepository<T> where T : class
{
    void Add(T entity, bool considerLinks = true);

    void AddRange(IEnumerable<T> entities, bool considerLinks = true);

    IEnumerable<T> GetAll();

    Task<IEnumerable<T>> GetAllAsync();

    T GetById(object id);

    Task<T> GetByIdAsync(object id);

    Task<bool> CheckExistByIdAsync(object id);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

    void Update(T entity, bool considerLinks = true);

    void UpdateRange(IEnumerable<T> entities, bool considerLinks = true);

    void Remove(T entity, bool considerLinks = false);

    void RemoveRange(IEnumerable<T> entities, bool considerLinks = false);

    int SaveChanges();

    Task<int> SaveChangesAsync();

    Task<IRepositoryTransaction> BeginTransactionAsync();

}
