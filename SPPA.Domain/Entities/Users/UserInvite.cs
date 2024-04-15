using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.WorkSpaces;

namespace SPPA.Domain.Entities.Users;

public class UserInvite
{
    public Guid Id { get; set; }

    public string InviteCode { get; set; }

    public string UserEmail { get; set; }

    public Guid? WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }

    public UserRoleTypeEnum RoleType { get; set; }

    private UserInvite()
    {
        Id = Guid.Empty;
        InviteCode = Guid.NewGuid().ToString("N").ToLowerInvariant();
    }

    public UserInvite(
        string userEmail,
        UserRoleTypeEnum roleType
    )
        : this()
    {
        UserEmail = userEmail;
        RoleType = roleType;
    }

}