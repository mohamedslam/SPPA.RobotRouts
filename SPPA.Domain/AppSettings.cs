namespace SPPA.Domain;

public class AppSettings
{
    public string ServerName { get; set; }

    public AuthorizationSettings Authorization { get; set; }

    public EmailSettings EmailNotification { get; set; }

    public DatabaseSettings Database { get; set; }

    public ReportSetting? Report { get; set; }

    public LicensePlanLimit LicenseLimit { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ServerName))
        {
            throw new ArgumentNullException(nameof(ServerName));
        }

        if (Authorization == null)
        {
            throw new ArgumentNullException(nameof(Authorization));
        }
        Authorization.Validate();

        if (EmailNotification == null)
        {
            throw new ArgumentNullException(nameof(EmailNotification));
        }
        EmailNotification.Validate();

        if (Database == null)
        {
            throw new ArgumentNullException(nameof(Database));
        }
        Database.Validate();

        if (LicenseLimit == null)
            LicenseLimit = new LicensePlanLimit();
    }

}

public class AuthorizationSettings
{
    /// <summary>
    /// Login for main admin. Main admin a created on first system start.
    /// </summary>
    public string? AdminEmail { get; set; }

    /// <summary>
    /// Password for main admin. Main admin a created on first system start.
    /// </summary>
    public string? AdminPassword { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? JwtSecretSid { get; set; }

    /// <summary>
    /// In minutes
    /// </summary>
    public int JwtExpirationTime { get; set; } = 60 * 24 * 7;

    public void Validate()
    {
    }
}

public class EmailSettings
{
    public string Address { get; set; }
    public string SmtpServer { get; set; }
    public int? SmtpPort { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }

    public void Validate()
    {
        if (Address == null)
        {
            throw new ArgumentNullException(nameof(Address));
        }
        if (SmtpServer == null)
        {
            throw new ArgumentNullException(nameof(SmtpServer));
        }
        if (SmtpPort == null)
        {
            throw new ArgumentNullException(nameof(SmtpPort));
        }
        if (Login == null)
        {
            throw new ArgumentNullException(nameof(Login));
        }
        if (Password == null)
        {
            throw new ArgumentNullException(nameof(Password));
        }
    }
}

public class DatabaseSettings
{
    /// <summary>
    /// Connection string.
    /// </summary>
    public string? Connection { get; set; }

    /// <summary>
    /// Logging of any action. Including all sql requests
    /// </summary>
    public bool Logging { get; set; } = false;

    public void Validate()
    {
        if (Connection == null)
        {
            throw new ArgumentNullException(nameof(Connection));
        }
    }
}

public class ReportSetting
{
    public string? LicenseKey { get; set; }
}

public class LicensePlanLimit
{
    /// <summary>
    /// Switch of whitelist mode for invites
    /// </summary>
    public bool EnableInviteWhitelist { get; set; } = false;

    /// <summary>
    /// Switch of whitelist mode for registrations new users
    /// </summary>
    public bool EnableRegistrationWhitelist { get; set; } = false;

    /// <summary>
    /// Registration allowed only for email in this list.
    /// </summary>
    public string[] EmailWhitelist { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Лимит редакторов на рабочее пространство
    /// </summary>
    public int DefaultEditorsLimit { get; set; } = 3;

    /// <summary>
    /// Лимит хранилища документов на рабочее пространство
    /// </summary>
    public double DefaultStorageLimit { get; set; } = 5;

    public PersonalLicenseLimit[] PersonalLimit { get; set; } = Array.Empty<PersonalLicenseLimit>();
}

public class PersonalLicenseLimit
{
    /// <summary>
    /// Id рабочего пространства
    /// </summary>
    public Guid WorkspaceId { get; set; } = Guid.Empty;

    /// <summary>
    /// Лимит редакторов на рабочее пространство
    /// </summary>
    public int EditorsLimit { get; set; } = 3;

    /// <summary>
    /// Лимит хранилища документов на рабочее пространство
    /// </summary>
    public double StorageLimit { get; set; } = 5;
}