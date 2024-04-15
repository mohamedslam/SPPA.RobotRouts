namespace SPPA.Domain.Entities.Users;

public enum LanguageSupportedEnum
{
    en,
    ru,
}

public static class LanguageSupportedExtensions
{
    public static string GetCultureName(this LanguageSupportedEnum lang)
    {
        switch (lang)
        {
            case LanguageSupportedEnum.en:
                return "en-US";
            case LanguageSupportedEnum.ru:
                return "ru-RU";
            default:
                throw new ArgumentOutOfRangeException($"Language {lang.ToString()} not supported");
        }
    }

}
