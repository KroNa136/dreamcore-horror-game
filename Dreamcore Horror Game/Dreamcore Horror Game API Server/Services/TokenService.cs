using DreamcoreHorrorGameApiServer.ConstantValues.TokenOptions;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace DreamcoreHorrorGameApiServer.Services;

public class TokenService : ITokenService
{
    public EmailVerificationToken CreateEmailVerificationToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(EmailVerificationTokenOptions.Length);

        return new EmailVerificationToken()
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(randomBytes).Replace('+', '_').Replace('/', '_'),
            ExpirationTimestamp = DateTime.UtcNow.Add(TimeSpan.FromDays(EmailVerificationTokenOptions.LifetimeDays))
        };
    }

    public string CreateAccessToken(string login, string role)
        => CreateJwtToken
        (
            login: login,
            role: role,
            issuer: AccessTokenOptions.Issuer,
            audience: AccessTokenOptions.Audience,
            lifetime: AccessTokenOptions.LifetimeMinutes,
            securityKey: AccessTokenOptions.SecurityKey
        );

    public string CreateRefreshToken(string login, string role)
        => CreateJwtToken
        (
            login: login,
            role: role,
            issuer: RefreshTokenOptions.Issuer,
            audience: RefreshTokenOptions.Audience,
            lifetime: RefreshTokenOptions.LifetimeMinutes,
            securityKey: RefreshTokenOptions.SecurityKey
        );

    private static string CreateJwtToken(string login, string role, string issuer, string audience, double lifetime, SecurityKey securityKey)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.Role, role),
        ];

        JwtSecurityToken jwt = new
        (
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(lifetime)),
            signingCredentials: new(securityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
