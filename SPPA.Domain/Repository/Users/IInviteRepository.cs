using SPPA.Domain.Entities.Users;

namespace SPPA.Domain.Repository.Users;

public interface IInviteRepository : IBaseRepository<UserInvite>
{
    Task<UserInvite> GetWorkspaceInviteAsync(Guid workspaceId, Guid inviteId);

    Task<UserInvite> GetOrderInviteAsync(Guid orderId, Guid inviteId);

    Task<UserInvite> GetUserOrderInvitationAsync(Guid orderId, Guid userId);
    Task<UserInvite> GetUserWorkSpaceInvitationAsync(Guid workspaceId, Guid userId);
}