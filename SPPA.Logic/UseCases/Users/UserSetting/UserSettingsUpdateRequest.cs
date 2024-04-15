using SPPA.Logic.Dto.Users;
using MediatR;

namespace SPPA.Logic.UseCases.Users.UserSetting;

public class UserSettingsUpdateRequest : IRequest<UserSettingsDto>
{

    public Guid UserId { get; }

    public UserSettingsDto Mode { get; }

    public UserSettingsUpdateRequest(Guid userId, UserSettingsDto model)
    {
        UserId = userId;
        Mode = model;
    }

}
