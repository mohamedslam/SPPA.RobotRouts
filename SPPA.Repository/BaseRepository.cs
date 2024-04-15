using SPPA.Database;
using System.Linq.Expressions;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly ILogger<BaseRepository<T>> _logger;

    public BaseRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<T>> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public void Add(T entity, bool considerLinks = true)
    {
        if (considerLinks)
        {
            _dbContext.Set<T>().Add(entity);
        }
        else
        {
            _dbContext.Entry<T>(entity).State = EntityState.Unchanged;
            _dbContext.Add<T>(entity);
        }
    }

    public void AddRange(IEnumerable<T> entities, bool considerLinks = true)
    {
        if (considerLinks)
        {
            _dbContext.Set<T>().AddRange(entities);
        }
        else
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry<T>(entity).State = EntityState.Unchanged;
                _dbContext.Add<T>(entity);
            }
        }
    }

    public IEnumerable<T> GetAll()
    {
        return _dbContext.Set<T>()
                         .AsNoTracking()
                         .ToList();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>()
                               .AsNoTracking()
                               .ToListAsync();
    }

    public T GetById(object id)
    {
        T? record = _dbContext.Set<T>().Find(id);
        if (record == null)
            throw new MfNotFoundException("Record not found. Id: " + id.ToString());

        return record;
    }

    public async Task<T> GetByIdAsync(object id)
    {
        T? record = await _dbContext.Set<T>().FindAsync(id);
        if (record == null)
            throw new MfNotFoundException("Record not found. Id: " + id.ToString());

        return record;
    }

    public async Task<bool> CheckExistByIdAsync(object id)
    {
        T? record = await _dbContext.Set<T>().FindAsync(id);
        return record != null;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbContext.Set<T>()
                               .AsNoTracking()
                               .Where(expression)
                               .ToArrayAsync();
    }

    public void Update(T entity, bool considerLinks = true)
    {
        if (considerLinks)
        {
            _dbContext.Set<T>().Update(entity);
        }
        else
        {
            _dbContext.Entry<T>(entity).State = EntityState.Unchanged;
            _dbContext.Update<T>(entity);
        }
    }

    public void UpdateRange(IEnumerable<T> entities, bool considerLinks = true)
    {
        if (considerLinks)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }
        else
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry<T>(entity).State = EntityState.Unchanged;
                _dbContext.Update<T>(entity);
            }
        }
    }

    public void Remove(T entity, bool considerLinks = false)
    {
        if (considerLinks)
        {
            _dbContext.Set<T>().Remove(entity);
        }
        else
        {
            _dbContext.Entry<T>(entity).State = EntityState.Unchanged;
            _dbContext.Remove<T>(entity);
        }
    }

    public void RemoveRange(IEnumerable<T> entities, bool considerLinks = false)
    {
        if (considerLinks)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }
        else
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Unchanged;
                _dbContext.Remove(entity);
            }
        }
    }

    public int SaveChanges()
    {
        var count = _dbContext.SaveChanges();
        _logger.LogInformation($"Processed {count} DB records.");
        return count;
    }

    public async Task<int> SaveChangesAsync()
    {
        var count = await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Processed {count} DB records.");
        return count;
    }

    public async Task<IRepositoryTransaction> BeginTransactionAsync()
    {
        return await RepositoryTransaction.CreateAsync(_dbContext);
    }

}
