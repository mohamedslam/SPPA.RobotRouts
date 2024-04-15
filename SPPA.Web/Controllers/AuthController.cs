using SPPA.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SPPA.Logic.Dto.Auth;

namespace SPPA.Web.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/auth")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserAuthenticationService _userAuthenticationService;

    public AuthController(
        ILogger<AuthController> logger,
        UserAuthenticationService userAuthenticationService
    )
    {
        _logger = logger;
        _userAuthenticationService = userAuthenticationService;
    }

    [HttpPost("registration")]
    [AllowAnonymous]
    public async Task<IActionResult> Registration([FromBody] LoginDto dto)
    {
        await _userAuthenticationService.RegistrationNewUserAsync(dto);
        return Ok();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<LoginResponseDto> Login([FromBody] LoginDto dto)
    {
        await _userAuthenticationService.ValidateUserAsync(dto);
        return await _userAuthenticationService.CreateJwtTokenAsync();
    }

    [HttpPost("confirm/email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
    {
        await _userAuthenticationService.ConfirmEmailAsync(dto);
        return Ok();
    }

    [HttpPost("confirm/invite")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmInvite([FromBody] ConfirmInviteDto dto)
    {
        await _userAuthenticationService.ConfirmInviteAsync(dto);
        return Ok();
    }

    [HttpPost("password-reset")]
    [AllowAnonymous]
    public async Task<IActionResult> PasswordReset([FromBody] ForgotPasswordDto dto)
    {
        await _userAuthenticationService.ForgotPasswordAsync(dto);
        return Ok();
    }

    [HttpPost("confirm/password-reset")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPasswordReset([FromBody] ResetPasswordDto dto)
    {
        await _userAuthenticationService.ResetPasswordAsync(dto);
        return Ok();
    }

}

