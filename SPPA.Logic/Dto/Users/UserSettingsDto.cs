using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Users;

public class UserSettingsDto
{
    public LanguageSupportedEnum InterfaceLanguage { get; set; }

    public LanguageSupportedEnum ReportLanguage { get; set; }
}
