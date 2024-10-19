using System;

namespace SomethingFishy.Collabothon2024.Common.Models;

public sealed class CommerzCredentials
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public int RefreshExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public string TokenType { get; set; }
    public string SessionState { get; set; }
    public string Scope { get; set; }
}

public sealed class CommerzStampedCredentials
{
    public DateTimeOffset ServerTime { get; init; }
    public CommerzCredentials Credentials { get; init; }
}
