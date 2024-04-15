using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.Auth;

public class LoginResponseDto
{
    [Required]
    public string Token { get; set; }

    public DateTimeOffset Expiration { get; set; }

    public LoginResponseDto(string token, DateTimeOffset expiration)
    {
        Token = token;
        Expiration = expiration;
    }
}

