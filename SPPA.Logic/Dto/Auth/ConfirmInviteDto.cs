using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.Auth;

public class ConfirmInviteDto
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    public string Password { get; set; }
}

