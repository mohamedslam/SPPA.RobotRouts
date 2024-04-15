using SPPA.Database;
using SPPA.Domain.Entities.Orders.Files;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Repository.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository.Orders;

public class OrderFileRepository : BaseRepository<OrderFile>, IOrderFileRepository
{
    private const string FileStorageFolderName = "FileStorage";
    private const string UserFilesFolderName = "UserFiles";

    public OrderFileRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<OrderFile>> logger
    )
        : base(dbContext, logger)
    {
    }

    public async Task AddFileAsync(OrderFile orderFile, string tmpFilePath)
    {
        var workspaceId = await GetWorkspaceIdAsync(orderFile.OrderId);
        var filePath = Path.Combine
        (
            FileStorageFolderName,
            UserFilesFolderName,
            workspaceId.ToString(),
            orderFile.OrderId.ToString(),
            orderFile.FileType.ToString(),
            orderFile.Id.ToString()
        );

        try
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(tmpFilePath, filePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        orderFile.SetFilePath(filePath);
        await _dbContext.OrderFiles.AddAsync(orderFile);
    }

    public async Task<IEnumerable<OrderFile>> GetAllForOrderAsync(
        Guid orderId,
        OrderFileTypeEnum? fileType = null,
        bool includeRefs = false
    )
    {
        try
        {
            var query = _dbContext.OrderFiles
                                  .AsNoTracking()
                                  .Where(x => x.OrderId == orderId);

            if (fileType.HasValue)
            {
                query = query.Where(x => x.FileType == fileType.Value);
            }

            if (includeRefs)
            {
                query = query.Include(x => x.ElementsRef);
            }

            var list = await query.ToArrayAsync();
            return list;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read file list");
            throw;
        }
    }

    public async Task<OrderFile> GetFileAsync(Guid orderId, Guid fileId)
    {
        var result = await _dbContext.OrderFiles
                                     .Where(x => x.OrderId == orderId)
                                     .Where(x => x.Id == fileId)
                                     .SingleOrDefaultAsync();
        if (result == null)
            throw new MfNotFoundException($"File not found. OrderId:{orderId} FileId:{fileId}");

        return result;
    }

    public async Task DeleteFileAsync(Guid orderId, Guid fileId)
    {
        var file = await GetFileAsync(orderId, fileId);
        DeleteFile(file);
        await _dbContext.SaveChangesAsync();
    }

    public void DeleteFile(OrderFile file)
    {
        try
        {
            File.Delete(file.FilePath);
            _dbContext.OrderFiles.Remove(file);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed on delete file. FileId:" + file.Id);
            throw;
        }
    }

    #region File element Ref

    public void AddFileElementRefs(IEnumerable<OrderFileElementRef> entities)
    {
        var entitiesArray = entities as OrderFileElementRef[] ?? entities.ToArray();

        foreach (var entity in entitiesArray)
        {
            _dbContext.Entry(entity).State = EntityState.Unchanged;
            _dbContext.Add(entity);
        }
    }

    public void RemoveFileElementRefs(IEnumerable<OrderFileElementRef> entities)
    {
        var entitiesArray = entities as OrderFileElementRef[] ?? entities.ToArray();

        foreach (var entity in entitiesArray)
        {
            _dbContext.Entry(entity).State = EntityState.Unchanged;
            _dbContext.Remove(entity);
        }
    }

    #endregion

    #region Helpers methods

    private async Task<Guid> GetWorkspaceIdAsync(Guid orderId)
    {
        var workspaceId = await _dbContext.Orders
                                          .Where(x => x.Id == orderId)
                                          .Select(x => new { x.WorkspaceId })
                                          .SingleOrDefaultAsync();

        return workspaceId?.WorkspaceId ?? throw new MfNotFoundException("Order not found. OrderId:" + orderId);
    }

    #endregion

}

