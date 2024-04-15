using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.Auth;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
