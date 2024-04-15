using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.Users;

public class UserDto
{
    public Guid UserId { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
