using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookTracker.Api.Application.Auth.Login;
using BookTracker.Api.Domain.Members;
using Microsoft.IdentityModel.Tokens;

namespace BookTracker.Api.Security;

public class JwtTokenGenerator(JwtSettings settings)
{
    public LoginResponse Generate(Member member)
    {
        var expiresAt =
            DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);

        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    member.Id.ToString()),
                new(
                    ClaimTypes.Name,
                    member.Name.Value),
                new(
                    ClaimTypes.Email,
                    member.Email.Value),
                new(
                    ClaimTypes.Role,
                    member.Role.ToString())
            };

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(settings.SigningKey));

        var credentials =
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

        var value =
            new JwtSecurityTokenHandler().WriteToken(token);

        return
            new LoginResponse
            {
                AccessToken = value,
                ExpiresAt = expiresAt
            };
    }
}