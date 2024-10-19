using System.ComponentModel.DataAnnotations;

namespace SomethingFishy.Collabothon2024.API.Data;

public sealed class JwtConfiguration
{
    [Required, MinLength(32)]
    public byte[] Key { get; set; }

    [Required]
    public string Issuer { get; set; }

    [Required]
    public string Audience { get; set; }
}

