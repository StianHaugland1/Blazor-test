using System.Security.Claims;

namespace BlazorIntAuto.Common.Extentions;
public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(x => x.Type == "db_id")?.Value ?? string.Empty;
    }
}
