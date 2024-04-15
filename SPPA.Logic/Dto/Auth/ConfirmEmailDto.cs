using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.Auth;

public class ConfirmEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }
}

