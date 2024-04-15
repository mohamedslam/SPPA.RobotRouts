using SPPA.Database;
using SPPA.Domain.Entities.WorkSpaces;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Repository.Workspaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository.Workspaces;

public class WorkspaceRepository : BaseRepository<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<Workspace>> logger
    )
        : base(dbContext, logger)
    {
    }

    public async Task<IEnumerable<Workspace>> GetAllForUserAsync(Guid userId)
    {
        try
        {
            var list = await _dbContext.Workspaces
                                       .AsNoTracking()
                                       .Where(w => w.Roles
                                                    .Select(r => r.UserId)
                                                    .Contains(userId) ||
                                                   w.Orders
                                                    .SelectMany(o => o.Roles.Select(r => r.UserId))
                                                    .Contains(userId)
                                       )
                                       .ToArrayAsync();

            return list;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read Workspace list entity");
            throw;
        }
    }

    public async Task<Workspace> GetWorkspaceWitRolesAsync(Guid workspaceId)
    {
        try
        {
            var model = await _dbContext.Workspaces
                                        .AsNoTrackingWithIdentityResolution()
                                        .Where(x => x.Id == workspaceId)
                                        .Include(x => x.Roles)
                                        .ThenInclude(x => x.User)
                                        .Include(x => x.Orders)
                                        .ThenInclude(x => x.Roles)
                                        .AsSplitQuery()
                                        .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException("Workspace not found. Id: " + workspaceId);
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read Workspace entity. Id: " + workspaceId);
            throw;
        }
    }



}

