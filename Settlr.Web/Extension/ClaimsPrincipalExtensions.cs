using System.Security.Claims;

namespace Settlr.Web.Extension;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        string? idValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out Guid id) ? id : null;
    }
}
