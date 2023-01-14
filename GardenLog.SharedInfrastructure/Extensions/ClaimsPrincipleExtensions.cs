using System.Security.Claims;

namespace GardenLog.SharedInfrastructure.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUserProfileId(this ClaimsPrincipal principal)
    {
        if (principal == null) return "up1";
        // throw new ArgumentNullException(nameof(principal));

        var userName = principal.FindFirstValue(ClaimTypes.Sid);

        return userName ?? "up1";
    }
}
