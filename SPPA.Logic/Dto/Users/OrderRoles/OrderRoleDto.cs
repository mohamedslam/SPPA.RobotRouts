using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.OrderRoles;

public class OrderRoleDto
{
    public Guid RoleId { get; set; }

    public UserRoleTypeEnum? WorkspaceRoleType { get; set; }

    public UserRoleTypeEnum? RoleType { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }

    [EmailAddress]
    public string UserEmail { get; set; }

    private OrderRoleDto()
    {
    }

}
