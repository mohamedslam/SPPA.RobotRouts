using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPPA.Web.Identity;
using SPPA.Logic.Dto.Users;
using SPPA.Logic.UseCases.Users;
using SPPA.Logic.UseCases.Users.UserSetting;
using MediatR;

namespace SPPA.Web.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(
        ILogger<UserController> logger,
        IMediator mediator
    )
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<UserDto> GetUserInfo()
    {
        var userId = this.HttpContext.User.GetUserId();
        return await _mediator.Send(new UserInfoRequest(userId));
    }

    #region User settings

    [HttpGet("user-settings")]
    public async Task<UserSettingsDto> GetUserSettings()
    {
        var userId = this.HttpContext.User.GetUserId();
        var result = await _mediator.Send(new UserSettingsRequest(userId));
        return result;
    }

    [HttpPut("user-settings")]
    public async Task<UserSettingsDto> UpdateUserSettings(
        [FromBody] UserSettingsDto dto
    )
    {
        var userId = this.HttpContext.User.GetUserId();
        var result = await _mediator.Send(new UserSettingsUpdateRequest(userId, dto));
        return result;
    }

    #endregion

}
