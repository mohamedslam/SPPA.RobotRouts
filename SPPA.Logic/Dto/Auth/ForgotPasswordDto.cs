using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.Auth;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

