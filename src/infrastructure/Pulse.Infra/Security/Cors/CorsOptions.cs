using System.Collections.Generic;

namespace Pulse.Infra.Security.Cors;

public class CorsOptions
{
    public const string Section = "Cors";

    public List<string>? AllowedOrigins { get; init; }
}