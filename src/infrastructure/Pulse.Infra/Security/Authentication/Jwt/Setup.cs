using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Infra.Database;
using Throw;

namespace Pulse.Infra.Security.Authentication.Jwt;

public static class Setup
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureAndValidate<JwtOptions>(JwtOptions.Section, configuration);
        services.AddSingleton<IJwtProvider, JwtProvider>();
        
        var options = configuration.GetSection(JwtOptions.Section).Get<JwtOptions>();
        options.ThrowIfNull();
        
        // JwtBearerDefaults.AuthenticationScheme
        services.AddAuthentication()
            .AddJwtBearer(bearerOptions =>
            {
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidTypes = [JwtConstants.HeaderType],

                    ValidIssuer = options.Issuer,
                    ValidateIssuer = true,

                    ValidAudience = options.Audience,
                    ValidateAudience = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret!)),
                    ValidateIssuerSigningKey = true,

                    ValidateLifetime = true,

                    RequireSignedTokens = true,
                    RequireExpirationTime = true,

                    // No need to set clock drift if token is issued and verified by the same server
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}