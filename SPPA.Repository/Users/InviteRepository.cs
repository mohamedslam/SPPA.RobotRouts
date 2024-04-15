using SPPA.Database;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Repository.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository.Users;

public class InviteRepository : BaseRepository<UserInvite>, IInviteRepository
{
    public InviteRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<UserInvite>> logger
    )
        : base(dbContext, logger)
    {
    }

    public async Task<UserInvite> GetWorkspaceInviteAsync(Guid workspaceId, Guid inviteId)
    {
        try
        {
            var model = await _dbContext.Invites.Where(x => x.Id == inviteId
                                                   && x.WorkspaceId == workspaceId)
                                  .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException($"UserInvite not found. WorkspaceId:{workspaceId} InviteId:{inviteId}");
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read UserInvite entity." +
                                $" WorkspaceId:{workspaceId} InviteId:{inviteId}");
            throw;
        }
    }

    public async Task<UserInvite> GetOrderInviteAsync(Guid orderId, Guid inviteId)
    {
        try
        {
            var model = await _dbContext.Invites.Where(x => x.Id == inviteId
                                                         && x.OrderId == orderId)
                                        .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException($"UserInvite not found. OrderId:{orderId} InviteId:{inviteId}");
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read UserInvite entity." +
                                $" OrderId:{orderId} InviteId:{inviteId}");
            throw;
        }
    }

    public async Task<UserInvite> GetUserOrderInvitationAsync(Guid orderId, Guid userId)
    {
        try
        {
            var user = await _dbContext.Users.Where(u=>u.Id== userId)
                .SingleOrDefaultAsync(); ;
            var model = await _dbContext.Invites.Where(x => x.UserEmail  == user.Email
                                                         && x.OrderId == orderId)
                                        .Include(o => o.Order)
                                        .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException($"UserInvite not found. OrderId:{orderId} userId:{userId}");
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read UserInvite entity." +
                                $" OrderId:{orderId} userId:{userId}");
            throw;
        }
    }
    public async Task<UserInvite> GetUserWorkSpaceInvitationAsync(Guid workspaceId, Guid userId)
    {
        try
        {
            var user = await _dbContext.Users.Where(u => u.Id == userId)
                .SingleOrDefaultAsync(); ;
            var model = await _dbContext.Invites.Where(x => x.UserEmail == user.Email
                                                         && x.WorkspaceId == workspaceId)              
                                         .Include(w=>w.Workspace)                                       
                                         .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new MfNotFoundException($"UserInvite not found. workspaceId:{workspaceId} userId:{userId}");
            }

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read UserInvite entity." +
                                $" workspaceId:{workspaceId} userId:{userId}");
            throw;
        }
    }

}