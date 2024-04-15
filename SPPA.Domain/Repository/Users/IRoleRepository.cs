using SPPA.Domain.Entities.Users;

namespace SPPA.Domain.Repository.Users;

public interface IRoleRepository : IBaseRepository<UserRole>
{
    // Workspace
    Task<UserRole> GetRoleWithUsersForWorkspaceAsync(Guid workspaceId, Guid roleId);

    Task<UserRoleTypeEnum?> GetRoleForWorkspaceAsync(Guid workspaceId, Guid userId);

    Task<UserRoleTypeEnum?> GetRoleForWorkspaceInheritedByOrderAsync(Guid workspaceId, Guid userId);

    Task<IEnumerable<UserRole>> GetRolesWithUsersForWorkspaceAsync(Guid workspaceId);

    // Order
    Task<UserRole> GetRoleWithUsersForOrderAsync(Guid orderId, Guid roleId);

    Task<UserRoleTypeEnum?> GetRoleForOrderAsync(Guid orderId, Guid userId);

    Task<IEnumerable<UserRole>> GetRolesWithUsersForOrderAsync(Guid orderId);

}
