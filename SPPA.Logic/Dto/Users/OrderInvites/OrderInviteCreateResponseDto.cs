using SPPA.Logic.Dto.Users.OrderRoles;

namespace SPPA.Logic.Dto.Users.OrderInvites;

public class OrderInviteCreateResponseDto
{
    public OrderInviteDto? Invite { get; set; }

    public OrderRoleDto? Role { get; set; }
}

