using System;

namespace SomethingFishy.Collabothon2024.API.Data;

public readonly record struct UserTokenInfo(string AuthenticationToken, string RefreshToken, DateTimeOffset TokenExpiresAt);
