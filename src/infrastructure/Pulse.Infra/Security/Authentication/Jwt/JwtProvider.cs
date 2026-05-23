using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Aggregates.Users;
using Throw;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Pulse.Infra.Security.Authentication.Jwt;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;
    
    private readonly SigningCredentials _signingCredentials;
    
    public JwtProvider(IOptions<JwtOptions> options, IDateTimeProvider dateTimeProvider)
    {
        _options = options.Value;

        _options.ThrowIfNull();
        _options.Secret.ThrowIfNull();

        _dateTimeProvider = dateTimeProvider;

        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
            SecurityAlgorithms.HmacSha256);
    }

    public string Create(User user)
    {
        var userSecurityStamp = user.SecurityStamp.Value;
        userSecurityStamp.ThrowIfNull();
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(type: UserClaims.UserId, value: user.Id.Value),
            new(type: UserClaims.UserSecurityStamp, value: userSecurityStamp)
        };
        
        // TODO - add optional organization level claims
        
        var securityToken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            expires: _dateTimeProvider.UtcNow.AddMinutes(_options.ExpiryInMinutes),
            claims: claims,
            signingCredentials: _signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}