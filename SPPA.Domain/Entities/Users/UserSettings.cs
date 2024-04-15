using SPPA.Domain.Interfaces;

namespace SPPA.Domain.Entities.Users;

public partial class UserSettings : IEntityGuid
{
    /// <summary>
    /// UserId
    /// </summary>
    public Guid Id { get; set; }

    public User User { get; set; }

    public LanguageSupportedEnum InterfaceLanguage { get; set; }

    public LanguageSupportedEnum ReportLanguage { get; set; }

    [Obsolete("Only for mapper or EF", true)]
    private UserSettings()
    {
    }

    public UserSettings(Guid id)
    {
        Id = id;
        InterfaceLanguage = LanguageSupportedEnum.en;
        ReportLanguage = LanguageSupportedEnum.en;
    }
}
