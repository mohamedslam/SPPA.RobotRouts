using System.Security.Claims;
using SPPA.Domain.Exceptions;

namespace SPPA.Web.Identity;

public static class IdentityExtension
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr == null)
        {
            throw new MfBadRequestException("UserId not contains in claims");
        }

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            throw new MfBadRequestException("UserId is not UUID");
        }

        return userId;
    }
}


