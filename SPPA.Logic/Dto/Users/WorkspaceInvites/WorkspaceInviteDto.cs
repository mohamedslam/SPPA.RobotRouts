using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.WorkspaceInvites;

public class WorkspaceInviteDto
{
    public Guid InviteId { get; set; }

    public UserRoleTypeEnum RoleType { get; set; }

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }
}

