using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.OrderInvites;

public class OrderInviteDto
{
    public Guid InviteId { get; set; }

    public UserRoleTypeEnum RoleType { get; set; }

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }
}

