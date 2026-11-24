using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TaskDoc.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    /// <summary>The signed-in user's Users.Id (the "sub" claim).</summary>
    public static long GetUserId(this ClaimsPrincipal principal) =>
        long.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("The access token has no 'sub' claim."));
}
