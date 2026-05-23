using System.ComponentModel.DataAnnotations;

namespace Pulse.Infra.Security.Authentication.Jwt;

public class JwtOptions
{
    public const string Section = "Jwt";

    [Required] public string? Secret { get; set; }

    [Range(1, 1440)] public uint ExpiryInMinutes { get; set; }

    [Required] public string? Issuer { get; set; }

    [Required] public string? Audience { get; set; }
}