using Microsoft.AspNetCore.Identity;

namespace SPPA.Web.Identity;

public class LowerLookupNormalizer : ILookupNormalizer
{
    public string NormalizeName(string name)
    {
        return name.ToLowerInvariant();
    }

    public string NormalizeEmail(string email)
    {
        return email.ToLowerInvariant();
    }
}
