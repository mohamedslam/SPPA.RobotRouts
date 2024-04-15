using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.WorkSpaces;

namespace SPPA.Domain.Entities.Users;

public class UserRole
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid? WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }

    public UserRoleTypeEnum RoleType { get; set; }

    private UserRole()
    {
    }

    public UserRole(
        Guid userId,
        UserRoleTypeEnum roleType,
        Guid? workspaceId = null,
        Guid? orderId = null
    )
    {
        UserId = userId;
        WorkspaceId = workspaceId;
        OrderId = orderId;
        RoleType = roleType;
    }
}