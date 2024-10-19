using System;
using Microsoft.AspNetCore.Http;
using SomethingFishy.Collabothon2024.API.Data;
using SomethingFishy.Collabothon2024.API.Services;

namespace SomethingFishy.Collabothon2024.API;

internal static class Extensions
{
    public static bool TryGetCommerzCredentials(this HttpContext ctx, out UserTokenInfo tokenInfo)
    {
        tokenInfo = new();

        if (!ctx.User.Identity.IsAuthenticated)
            return false;

        var token = default(string);
        var refresh = default(string);
        var expires = default(DateTimeOffset?);
        foreach (var claim in ctx.User.Claims)
        {
            switch (claim.Type)
            {
                case AuthenticationTokenHandler.ClaimTypeCommerzToken:
                    token = claim.Value;
                    break;

                case AuthenticationTokenHandler.ClaimTypeCommerzRefreshToken:
                    refresh = claim.Value;
                    break;

                case AuthenticationTokenHandler.ClaimTypeCommerzTokenExpiry:
                    expires = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(claim.Value));
                    break;
            }
        }

        if (token is null || refresh is null || expires is null)
            return false;

        tokenInfo = new(token, refresh, expires.Value);
        return true;
    }
}
