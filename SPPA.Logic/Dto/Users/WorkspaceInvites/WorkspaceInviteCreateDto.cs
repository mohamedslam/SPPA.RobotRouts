using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.WorkspaceInvites;

public class WorkspaceInviteCreateDto
{
    public UserRoleTypeEnum RoleType { get; set; }

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }

    private WorkspaceInviteCreateDto()
    {
    }
}

