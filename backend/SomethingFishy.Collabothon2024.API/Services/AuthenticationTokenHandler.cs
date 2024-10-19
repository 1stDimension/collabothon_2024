using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SomethingFishy.Collabothon2024.API.Data;
using SomethingFishy.Collabothon2024.Common.Models;

namespace SomethingFishy.Collabothon2024.API.Services;

public sealed class AuthenticationTokenHandler
{
    public const string ClaimTypeCommerzToken = "commerz-token";
    public const string ClaimTypeCommerzTokenExpiry = "commerz-token-expires";
    public const string ClaimTypeCommerzRefreshToken = "commerz-refresh";

    private readonly JwtConfiguration _config;

    public AuthenticationTokenHandler(IOptions<JwtConfiguration> options)
    {
        this._config = options.Value;
    }

    public string Issue(CommerzStampedCredentials commerz)
    {
        var securityKey = new SymmetricSecurityKey(this._config.Key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var commerzCredentials = commerz.Credentials;
        var expiryToken = commerz.ServerTime.AddSeconds(commerzCredentials.ExpiresIn);
        var expiryRefresh = commerz.ServerTime.AddSeconds(commerzCredentials.RefreshExpiresIn);

        var token = new JwtSecurityToken(
            this._config.Issuer,
            this._config.Audience,
            claims: [
                new(ClaimTypeCommerzToken, commerzCredentials.AccessToken),
                new(ClaimTypeCommerzTokenExpiry, expiryToken.ToUnixTimeMilliseconds().ToString()),
                new(ClaimTypeCommerzRefreshToken, commerzCredentials.RefreshToken),
            ],
            notBefore: DateTime.UtcNow,
            expires: expiryRefresh.DateTime,
            credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}

