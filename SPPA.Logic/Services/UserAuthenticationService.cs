using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using SPPA.Domain;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using SPPA.Logic.Extensions;
using SPPA.Logic.Dto.Auth;
using SPPA.Logic.Dto.WorkSpaces;
using Microsoft.Extensions.Logging;
using SPPA.Domain.Repository.Users;

namespace SPPA.Logic.Services;

public class UserAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;
    private readonly EmailService _emailService;
    private readonly WorkspaceService _workspaceService;
    private readonly ILogger<UserAuthenticationService> _logger;
    private readonly RoleService _roleService;
    private readonly LicensePlanLimitationsService _licenseLimitService;
    private readonly IInviteRepository _inviteRepository;

    private User? _user;

    public UserAuthenticationService(
        UserManager<User> userManager,
        IOptions<AppSettings> appSettings,
        IMapper mapper,
        EmailService emailService,
        WorkspaceService workspaceService,
        ILogger<UserAuthenticationService> logger,
        RoleService roleService,
        LicensePlanLimitationsService licenseLimitService,
        IInviteRepository inviteRepository
    )
    {
        _userManager = userManager;
        _appSettings = appSettings.Value;
        _mapper = mapper;
        _emailService = emailService;
        _workspaceService = workspaceService;
        _logger = logger;
        _roleService = roleService;
        _licenseLimitService = licenseLimitService;
        _inviteRepository = inviteRepository;
    }

    public async Task RegistrationNewUserAsync(LoginDto registrationDto)
    {
        _licenseLimitService.CheckEmailForRegistration(registrationDto.Email);

        var user = new User(registrationDto.Email);
        var identityResult = await _userManager.CreateAsync(user, registrationDto.Password);
        if (!identityResult.Succeeded)
            throw new MfBadRequestException(identityResult.Errors.ErrorsToString());

        var confirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodeCode = WebUtility.UrlEncode(confirmCode);

        await SendRegistrationConfirmMessageAsync(user.Email, encodeCode);
    }

    private async Task SendRegistrationConfirmMessageAsync(string email, string code)
    {
        var confirmUrl = _appSettings.ServerName + $@"/auth/confirm-email?email={email}&code={code}";
        await _emailService.SendEmailAsync(
            email,
            "Подтвердите почту в МастерФаб",
            "Здравствуйте!" +
            "<br>" +
            "Пожалуйста, подтвердите свой адрес электронной почты по ссылке ниже. Это нужно, чтобы вы смогли получить доступ к сервису МастерФаб." +
            "<br>" +
            $"<a href='{confirmUrl}'>Подтвердить почту</a>." +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            "Please confirm your email address using the link below. This is necessary so that you can access the SPPA service." +
            "<br>" +
            $"<a href='{confirmUrl}'>Confirm email</a>." +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }

    #region Ligin user

    public async Task ValidateUserAsync(LoginDto loginDto)
    {
        _user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (_user == null || !(await _userManager.CheckPasswordAsync(_user, loginDto.Password)))
            throw new MfPermissionException("Incorrect login or password",
                "server-error.permission.incorrect-login-password");
        if (!_user.EmailConfirmed)
            throw new MfPermissionException("Please confirm your email address",
                "server-error.permission.confirm-email");
        if (_user.LockoutEnabled)
            throw new MfPermissionException("User is blocked",
                "server-error.permission.user-blocked");
    }

    public async Task<LoginResponseDto> CreateJwtTokenAsync()
    {
        var tokenOptions = await GenerateTokenOptionsAsync();
        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return new LoginResponseDto(token, tokenOptions.ValidTo);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secretSid = _appSettings.Authorization.JwtSecretSid;
        var key = Encoding.UTF8.GetBytes(secretSid);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaimsAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _user.Id.ToString()),
        };
        //var roles = await _userManager.GetRolesAsync(_user);
        //foreach (var role in roles)
        //{
        //    claims.Add(new Claim(ClaimTypes.Role, role));
        //}
        return claims;
    }

    private async Task<JwtSecurityToken> GenerateTokenOptionsAsync()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaimsAsync();
        var expirationTime = _appSettings.Authorization.JwtExpirationTime;

        var tokenOptions = new JwtSecurityToken
        (
            issuer: _appSettings.ServerName,
            //audience: _appSettings.ServerName,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expirationTime),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }

    #endregion

    public async Task ConfirmEmailAsync(ConfirmEmailDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Code))
            throw new MfBadRequestException("Empty Email or Code");

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new MfNotFoundException("User not found.");

        if (user.EmailConfirmed)
            throw new MfNotAcceptableException("User already confirm a email",
                "server-error.not-acceptable.email-already-confirm");

        var identityResult = await _userManager.ConfirmEmailAsync(user, dto.Code);
        if (!identityResult.Succeeded)
            throw new MfBadRequestException(identityResult.Errors.ErrorsToString());

        try
        {
            await _workspaceService.CreateAsync(new WorkspaceCreateUpdateDto(user.UserName), user.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on create default workspace");
        }

        await ApplyInvitesAsync(user);
    }

    public async Task ConfirmInviteAsync(ConfirmInviteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code) || string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new MfBadRequestException("Empty invite code or user email");
        }

        var invites = await _inviteRepository.FindAsync(x => x.UserEmail == dto.Email);
        if (!invites.Where(x => x.InviteCode == dto.Code).Any())
        {
            throw new MfBadRequestException("Invite is not valid");
        }

        var user = new User(dto.Email)
        {
            EmailConfirmed = true,
        };
        var identityResult = await _userManager.CreateAsync(user, dto.Password);
        if (!identityResult.Succeeded)
        {
            throw new MfBadRequestException(identityResult.Errors.ErrorsToString());
        }

        try
        {
            await _workspaceService.CreateAsync(new WorkspaceCreateUpdateDto(user.UserName), user.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on create default workspace");
        }

        await ApplyInvitesAsync(user);
    }

    private async Task ApplyInvitesAsync(User user)
    {
        var invites = await _inviteRepository.FindAsync(x => x.UserEmail == user.Email);
        try
        {
            foreach (var invite in invites)
            {
                if (invite.WorkspaceId.HasValue)
                {
                    try
                    {
                        await _roleService.CreateWorkspaceRoleAsync(invite.WorkspaceId.Value, user.Id, invite.RoleType);
                    }
                    catch (MfLicenseException e)
                    {
                        _logger.LogWarning(e, $"Not created {invite.RoleType} role fo user. {user.UserName}. Reason:" + e.Message);
                        await _roleService.CreateWorkspaceRoleAsync(invite.WorkspaceId.Value, user.Id, UserRoleTypeEnum.Viewer);
                    }
                }
                else if (invite.OrderId.HasValue)
                {
                    try
                    {
                        await _roleService.CreateOrderRoleAsync(invite.OrderId.Value, user.Id, invite.RoleType);
                    }
                    catch (MfLicenseException e)
                    {
                        _logger.LogWarning(e, $"Not created {invite.RoleType} role fo user. {user.UserName}. Reason:" + e.Message);
                        await _roleService.CreateOrderRoleAsync(invite.OrderId.Value, user.Id, UserRoleTypeEnum.Viewer);
                    }
                }
                else
                {
                    _logger.LogError($"Invite without WorkspaceId and OrderId. Invite.Id:{invite.Id}");
                    continue;
                }

                _inviteRepository.Remove(invite);
                await _inviteRepository.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on create role after email confirm.");
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || user.LockoutEnabled)
            return;

        var confirmCode = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodeCode = WebUtility.UrlEncode(confirmCode);

        await SendResetPassConfirmMessageAsync(user.Email, encodeCode);
    }

    private async Task SendResetPassConfirmMessageAsync(string email, string code)
    {
        var confirmUrl = _appSettings.ServerName + $@"/auth/confirm-reset-password?email={email}&code={code}";
        await _emailService.SendEmailAsync(
            email,
            "Сброс пароля в МастерФаб",
            "Здравствуйте!" +
            "<br>" +
            "Вы запросили восстановление пароля." +
            "<br>" +
            $"Перейдите по ссылке, чтобы сбросить пароль: <a href='{confirmUrl}'>сбросить</a>." +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            "You have requested password recovery." +
            "<br>" +
            $"Follow the link to reset your password: <a href='{confirmUrl}'>reset</a>." +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new MfNotFoundException("User not found.");
        }

        var identityResult = await _userManager.ResetPasswordAsync(user, dto.Code, dto.Password);
        if (!identityResult.Succeeded)
        {
            throw new MfBadRequestException(identityResult.Errors.ErrorsToString());
        }

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }
    }

}
