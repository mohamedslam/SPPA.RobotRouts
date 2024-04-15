using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.WorkspaceRoles;

public class WorkspaceRoleDto
{
    public Guid RoleId { get; set; }

    public UserRoleTypeEnum RoleType { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }

    [EmailAddress]
    public string UserEmail { get; set; }

    private WorkspaceRoleDto()
    {
    }

}
