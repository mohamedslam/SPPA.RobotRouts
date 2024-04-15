using Microsoft.AspNetCore.Identity;

namespace SPPA.Logic.Extensions;

public static class IdentityExtension
{
    public static string ErrorsToString(this IEnumerable<IdentityError> identityErrors)
    {
        var errors = identityErrors.Select(x => x.Code + " " + x.Description);
        return string.Join(", ", errors);
    }
}
