using SPPA.Logic.Dto.Users;
using MediatR;

namespace SPPA.Logic.UseCases.Users;

public class UserInfoRequest : IRequest<UserDto>
{

    public Guid UserId { get; }

    public UserInfoRequest(Guid userId)
    {
        UserId = userId;
    }

}
