using SPPA.Domain.Entities.Orders;

namespace SPPA.Domain.Repository.Orders;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<Order> GetOrderWithRolesAndUsersAsync(Guid orderId);

    Task<IEnumerable<Order>> GetAllForUserAsync(Guid workspaceId, Guid userId);
}
