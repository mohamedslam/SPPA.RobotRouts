using SPPA.Database;
using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Repository.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository.Orders;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<Order>> logger
    )
        : base(dbContext, logger)
    {
    }

    public async Task<Order> GetOrderWithRolesAndUsersAsync(Guid orderId)
    {
        try
        {
            var model = await _dbContext.Orders
                                        .AsNoTrackingWithIdentityResolution()
                                        .Where(x => x.Id == orderId)
                                        .Include(x => x.Roles)
                                        .ThenInclude(x => x.User)
                                        .Include(x => x.Workspace)
                                        .ThenInclude(x => x.Roles)
                                        .ThenInclude(x => x.User)
                                        .AsSplitQuery()
                                        .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException($"Order not found. OrderId:{orderId}");
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Order entity. OrderId:{orderId}");
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetAllForUserAsync(Guid workspaceId, Guid userId)
    {
        try
        {

            var list = (await _dbContext.Orders
                                        .AsNoTracking()
                                        .Where(o => o.WorkspaceId == workspaceId)
                                        .Include(x => x.Roles.Where(r => r.UserId == userId))
                                        .Include(x => x.Workspace)
                                        .ThenInclude(x => x.Roles.Where(r => r.UserId == userId))
                                        .AsSplitQuery()
                                        .ToArrayAsync()
                       )
                       .Where(o => o.Roles.Any() || o.Workspace.Roles.Any())
                       .ToArray();

            return list;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Orders. WorkspaceId:{workspaceId} UserId{userId}");
            throw;
        }
    }

}

