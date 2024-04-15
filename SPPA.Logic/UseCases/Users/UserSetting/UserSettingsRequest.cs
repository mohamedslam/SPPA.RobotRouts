using SPPA.Logic.Dto.Users;
using MediatR;

namespace SPPA.Logic.UseCases.Users.UserSetting;

public class UserSettingsRequest : IRequest<UserSettingsDto>
{

    public Guid UserId { get; }

    public UserSettingsRequest(Guid userId)
    {
        UserId = userId;
    }

}
