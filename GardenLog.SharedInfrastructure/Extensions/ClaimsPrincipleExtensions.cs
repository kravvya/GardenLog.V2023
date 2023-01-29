using Microsoft.AspNetCore.Http;
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

    public static string GetUserProfileId(this ClaimsPrincipal principal, IHeaderDictionary headers)
    {
        var deafultuser = "";

        if (headers != null)
        {
            deafultuser = headers.FirstOrDefault(h => h.Key == "RequestUser").Value;
        }

        if (string.IsNullOrEmpty(deafultuser)) deafultuser = "up1";

        if (principal == null) return deafultuser;
        // throw new ArgumentNullException(nameof(principal));

        var userName = principal.FindFirstValue(ClaimTypes.Sid);

        return userName ?? deafultuser;
    }
}
