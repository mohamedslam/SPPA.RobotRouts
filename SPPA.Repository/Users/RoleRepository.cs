using SPPA.Database;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Repository.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository.Users;

public class RoleRepository : BaseRepository<UserRole>, IRoleRepository
{
    public RoleRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<UserRole>> logger
    )
        : base(dbContext, logger)
    {
    }

    #region Workspace

    public async Task<UserRole> GetRoleWithUsersForWorkspaceAsync(Guid workspaceId, Guid roleId)
    {
        try
        {
            var model = await _dbContext.UserRoles
                                        .AsNoTracking()
                                        .Where(x => x.Id == roleId && x.WorkspaceId == workspaceId)
                                        .Include(x => x.User)
                                        .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException($"Role not found. RoleId:{roleId}");
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Role entity. RoleId:{roleId}");
            throw;
        }
    }

    public async Task<UserRoleTypeEnum?> GetRoleForWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        try
        {
            var model = await _dbContext.Workspaces
                                        .AsNoTracking()
                                        .Where(x => x.Id == workspaceId)
                                        .SelectMany(x => x.Roles)
                                        .Where(x => x.UserId == userId)
                                        .Select(x => new { x.RoleType })
                                        .SingleOrDefaultAsync();

            return model?.RoleType; ;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Role entity. WorkspaceId:{workspaceId} UserId:{userId}");
            throw;
        }
    }

    public async Task<UserRoleTypeEnum?> GetRoleForWorkspaceInheritedByOrderAsync(Guid workspaceId, Guid userId)
    {
        try
        {
            var isOrdersAnyRoles = await _dbContext.Workspaces
                                                   .AsNoTracking()
                                                   .Where(x => x.Id == workspaceId)
                                                   .SelectMany(x => x.Orders)
                                                   .SelectMany(x => x.Roles)
                                                   .Where(x => x.UserId == userId)
                                                   .AnyAsync();

            return isOrdersAnyRoles ? UserRoleTypeEnum.Viewer : null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Role entity. WorkspaceId:{workspaceId} UserId:{userId}");
            throw;
        }
    }

    public async Task<IEnumerable<UserRole>> GetRolesWithUsersForWorkspaceAsync(Guid workspaceId)
    {
        try
        {
            var model = await _dbContext.UserRoles
                                        .AsNoTracking()
                                        .Where(x => x.WorkspaceId == workspaceId)
                                        .Include(x => x.User)
                                        .ToArrayAsync();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Roles entities. WorkspaceId:{workspaceId}");
            throw;
        }
    }


    #endregion

    #region Order

    public async Task<UserRole> GetRoleWithUsersForOrderAsync(Guid orderId, Guid roleId)
    {
        try
        {
            var model = await _dbContext.UserRoles
                                        .AsNoTracking()
                                        .Where(x => x.Id == roleId && x.OrderId == orderId)
                                        .Include(x => x.User)
                                        .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException($"Role not found. RoleId:{roleId}");
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Role entity. RoleId:{roleId}");
            throw;
        }
    }

    public async Task<UserRoleTypeEnum?> GetRoleForOrderAsync(Guid orderId, Guid userId)
    {
        try
        {
            Guid workspaceId;

            try
            {
                workspaceId = await _dbContext.Orders
                                              .AsNoTracking()
                                              .Where(x => x.Id == orderId)
                                              .Select(x => x.WorkspaceId)
                                              .SingleAsync();
            }
            catch (InvalidOperationException)
            {
                throw new MfNotFoundException("Order not found. OrderId:" + orderId);
            }

            var roles = await _dbContext.UserRoles
                                        .AsNoTracking()
                                        .Where(x => x.UserId == userId)
                                        .Where(x => x.OrderId == orderId || x.WorkspaceId == workspaceId)
                                        .ToArrayAsync();

            var orderRole = roles.Where(x => x.OrderId == orderId)
                                 .SingleOrDefault();

            var workspaceRole = roles.Where(x => x.WorkspaceId == workspaceId)
                                     .SingleOrDefault();

            return orderRole?.RoleType ?? workspaceRole?.RoleType ?? null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Role entity. OrderId:{orderId} UserId:{userId}");
            throw;
        }
    }

    public async Task<IEnumerable<UserRole>> GetRolesWithUsersForOrderAsync(Guid orderId)
    {
        try
        {
            var model = await _dbContext.UserRoles
                                        .AsNoTracking()
                                        .Where(x => x.OrderId == orderId)
                                        .Include(x => x.User)
                                        .ToArrayAsync();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on read Roles entities. OrderId:{orderId}");
            throw;
        }
    }

    #endregion

}

