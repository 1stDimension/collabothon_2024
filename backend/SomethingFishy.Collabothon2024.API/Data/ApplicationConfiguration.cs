using System.ComponentModel.DataAnnotations;

namespace SomethingFishy.Collabothon2024.API.Data;

public sealed class ApplicationConfiguration
{
    [Required]
    public string DatabaseFile { get; set; }

    public bool LogSensitive { get; set; }

    [Required]
    public string ClientId { get; set; }

    [Required]
    public string ClientSecret { get; set; }
}

