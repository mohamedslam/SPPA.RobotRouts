using SPPA.Domain.Entities.Orders.Files;

namespace SPPA.Domain.Repository.Orders;

public interface IOrderFileRepository : IBaseRepository<OrderFile>
{
    Task AddFileAsync(OrderFile orderFile, string tmpFilePath);
    Task<IEnumerable<OrderFile>> GetAllForOrderAsync(Guid orderId, OrderFileTypeEnum? fileType = null, bool includeRefs = false);
    Task<OrderFile> GetFileAsync(Guid orderId, Guid fileId);
    Task DeleteFileAsync(Guid orderId, Guid fileId);
    void DeleteFile(OrderFile file);
    void AddFileElementRefs(IEnumerable<OrderFileElementRef> entities);
    void RemoveFileElementRefs(IEnumerable<OrderFileElementRef> entities);
}
