using AutoMapper;
using SPPA.Domain.Repository.Users;
using SPPA.Logic.Dto.Users;
using MediatR;

namespace SPPA.Logic.UseCases.Users;

public class UserInfoRequestHandler
    : IRequestHandler<UserInfoRequest, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;


    public UserInfoRequestHandler(
        IUserRepository userRepository,
        IMapper mapper
    )
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UserInfoRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var model = _mapper.Map<UserDto>(user);
        return model;
    }

}

