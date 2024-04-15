using SPPA.Logic.Dto.Users.WorkspaceRoles;

namespace SPPA.Logic.Dto.Users.WorkspaceInvites;

public class WorkspaceInviteCreateResponseDto
{
    public WorkspaceInviteDto? Invite { get; set; }

    public WorkspaceRoleDto? Role { get; set; }
}

