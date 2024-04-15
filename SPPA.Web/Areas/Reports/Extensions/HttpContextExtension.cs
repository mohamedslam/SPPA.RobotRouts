using SPPA.Domain.Entities.Users;

namespace SPPA.Web.Areas.Reports.Extensions;

public static class HttpContextExtension
{
    public static LanguageSupportedEnum? GetLanguageFromCookie(this HttpContext context, string cookieName)
    {
        if (context.Request.Cookies.ContainsKey(cookieName))
        {
            var langStr = context.Request.Cookies[cookieName];
            if (Enum.TryParse<LanguageSupportedEnum>(langStr, true, out var lang))
            {
                return lang;
            }
        }

        return null;
    }

    public static LanguageSupportedEnum? GetBrowserLanguage(this HttpContext context)
    {
        var languages = context.Request.GetTypedHeaders().AcceptLanguage.Select(x => x.Value.Value);
        foreach (var language in languages)
        {
            if (language.ToLowerInvariant().StartsWith("ru"))
                return LanguageSupportedEnum.ru;
            if (language.ToLowerInvariant().StartsWith("en"))
                return LanguageSupportedEnum.en;
        }

        return null;
    }

}

