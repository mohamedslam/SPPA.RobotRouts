using AutoMapper;
using SPPA.Domain.Repository.Users;
using SPPA.Logic.Dto.Users;
using MediatR;

namespace SPPA.Logic.UseCases.Users.UserSetting;

public class UserSettingsRequestHandler
    : IRequestHandler<UserSettingsRequest, UserSettingsDto>
{
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly IMapper _mapper;

    public UserSettingsRequestHandler(
        IUserSettingsRepository userSettingsRepository,
        IMapper mapper
    )
    {
        _userSettingsRepository = userSettingsRepository;
        _mapper = mapper;
    }

    public async Task<UserSettingsDto> Handle(UserSettingsRequest request, CancellationToken cancellationToken)
    {
        var userSettings = await _userSettingsRepository.GetByIdAsync(request.UserId);
        var model = _mapper.Map<UserSettingsDto>(userSettings);
        return model;
    }

}
