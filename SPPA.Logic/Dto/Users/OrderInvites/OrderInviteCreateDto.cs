using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.OrderInvites;

public class OrderInviteCreateDto
{
    public UserRoleTypeEnum RoleType { get; set; }

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }

    private OrderInviteCreateDto()
    {
    }
}

