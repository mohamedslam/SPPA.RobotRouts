using SPPA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SPPA.Repository;

public class RepositoryTransaction : IRepositoryTransaction
{
    private IDbContextTransaction _dbContextTransaction;

    private RepositoryTransaction(IDbContextTransaction dbContextTransaction)
    {
        _dbContextTransaction = dbContextTransaction;
    }

    public static async Task<RepositoryTransaction> CreateAsync(DbContext dbContext)
    {
        var dbContextTransaction = await dbContext.Database.BeginTransactionAsync();
        var result = new RepositoryTransaction(dbContextTransaction);
        return result;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _dbContextTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _dbContextTransaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContextTransaction.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _dbContextTransaction.DisposeAsync();
    }
}
