using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TalyerApp.Api.Configuration;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Application.Dto;
using TalyerApp.Domain.Authorization;

namespace TalyerApp.Api.Identity;

public sealed class TalyerSessionTokenIssuer : ITalyerSessionTokenIssuer
{
    private readonly TalyerSessionOptions _options;

    public TalyerSessionTokenIssuer(IOptions<TalyerSessionOptions> options)
    {
        _options = options.Value;
    }

    public string Issue(SessionDto session)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, session.UserId.ToString()),
            new(TalyerClaimTypes.UserId, session.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, session.Email),
            new(ClaimTypes.Name, session.Username)
        };
        claims.AddRange(session.Assignments.Select(a => new Claim(ClaimTypes.Role, a.RoleCode)));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.LifetimeMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
