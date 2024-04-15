using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.WorkspaceRoles;

public class WorkspaceRoleUpdateDto
{
    public UserRoleTypeEnum RoleType { get; set; }

    private WorkspaceRoleUpdateDto()
    {
    }

}