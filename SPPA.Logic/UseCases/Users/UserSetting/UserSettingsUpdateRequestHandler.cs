using AutoMapper;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Repository.Users;
using SPPA.Logic.Dto.Users;
using MediatR;

namespace SPPA.Logic.UseCases.Users.UserSetting;

public class UserSettingsUpdateRequestHandler
    : IRequestHandler<UserSettingsUpdateRequest, UserSettingsDto>
{
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly IMapper _mapper;

    public UserSettingsUpdateRequestHandler(
        IUserSettingsRepository userSettingsRepository,
        IMapper mapper
    )
    {
        _userSettingsRepository = userSettingsRepository;
        _mapper = mapper;
    }

    public async Task<UserSettingsDto> Handle(UserSettingsUpdateRequest request, CancellationToken cancellationToken)
    {
        UserSettings userSettings;
        try
        {
            userSettings = await _userSettingsRepository.GetByIdAsync(request.UserId);
            _mapper.Map(request.Mode, userSettings);
            _userSettingsRepository.Update(userSettings);
        }
        catch (MfNotFoundException)
        {
            userSettings = new UserSettings(request.UserId);
            _mapper.Map(request.Mode, userSettings);
            _userSettingsRepository.Add(userSettings);
        }

        await _userSettingsRepository.SaveChangesAsync();
        var result = _mapper.Map<UserSettingsDto>(userSettings);
        return result;
    }

}
