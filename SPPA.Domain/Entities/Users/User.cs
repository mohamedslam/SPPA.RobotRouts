using SPPA.Domain.Interfaces;

namespace SPPA.Domain.Entities.Users;

public class User : IEntityGuid
{
    public Guid Id { get; set; }
    public string UserName { get; set; }

    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }

    public string PasswordHash { get; set; }
    public string SecurityStamp { get; set; }

    public string ConcurrencyStamp { get; set; }

    public List<UserRole> Roles { get; set; }

    public UserSettings UserSettings { get; set; }

    [Obsolete("Only for mapper or EF", true)]
    private User()
    {
        Roles = new List<UserRole>();
    }

    public User(string email)
    {
        Id = Guid.Empty;
        UserName = email;
        Email = email;
        EmailConfirmed = false;
        LockoutEnd = null;
        LockoutEnabled = false;
        AccessFailedCount = 0;

        ConcurrencyStamp = Guid.NewGuid().ToString();
        Roles = new List<UserRole>();
    }
}
