using System.ComponentModel.DataAnnotations;

namespace Pulse.Infra.Security.Authentication.Cookie;

public class CookieOptions
{
    public const string Section = "Cookie";

    public string? Name { get; set; }

    public bool UseStrict { get; set; }

    [Required] public string? Domain { get; set; }
    
    [Range(1, 14)] public uint ExpiryInDays { get; set; }
}