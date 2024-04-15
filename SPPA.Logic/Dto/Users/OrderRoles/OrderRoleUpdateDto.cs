using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users.OrderRoles;

public class OrderRoleUpdateDto
{
    public UserRoleTypeEnum RoleType { get; set; }

    private OrderRoleUpdateDto()
    {
    }

}
