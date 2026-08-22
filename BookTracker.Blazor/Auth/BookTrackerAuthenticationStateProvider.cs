using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookTracker.Blazor.Auth;

public sealed class BookTrackerAuthenticationStateProvider(
    IAuthSession authSession) : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous =
        new(new ClaimsIdentity());

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        var token = await authSession.GetTokenAsync();
        var user = CreatePrincipalOrAnonymous(token);

        return new AuthenticationState(user);
    }

    public async Task SignInAsync(string token)
    {
        await authSession.SetTokenAsync(token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SignOutAsync()
    {
        await authSession.ClearTokenAsync();
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(Anonymous)));
    }

    private static ClaimsPrincipal CreatePrincipalOrAnonymous(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Anonymous;
        }

        try
        {
            var segments = token.Split('.');

            if (segments.Length != 3)
            {
                return Anonymous;
            }

            var payloadBytes = DecodeBase64Url(segments[1]);
            using var document = JsonDocument.Parse(payloadBytes);
            var payload = document.RootElement;

            if (!payload.TryGetProperty("exp", out var expiration) ||
                expiration.ValueKind != JsonValueKind.Number ||
                !expiration.TryGetInt64(out var expiresAt) ||
                DateTimeOffset.FromUnixTimeSeconds(expiresAt) <=
                    DateTimeOffset.UtcNow)
            {
                return Anonymous;
            }

            var claims = new List<Claim>();
            AddClaim(payload, "nameid", ClaimTypes.NameIdentifier, claims);
            AddClaim(payload, "unique_name", ClaimTypes.Name, claims);
            AddClaim(payload, "email", ClaimTypes.Email, claims);
            AddClaim(payload, "role", ClaimTypes.Role, claims);

            AddClaim(payload, ClaimTypes.NameIdentifier, ClaimTypes.NameIdentifier, claims);
            AddClaim(payload, ClaimTypes.Name, ClaimTypes.Name, claims);
            AddClaim(payload, ClaimTypes.Email, ClaimTypes.Email, claims);
            AddClaim(payload, ClaimTypes.Role, ClaimTypes.Role, claims);

            if (!claims.Exists(claim =>
                    claim.Type == ClaimTypes.NameIdentifier))
            {
                return Anonymous;
            }

            var identity = new ClaimsIdentity(
                claims,
                authenticationType: "jwt",
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);

            return new ClaimsPrincipal(identity);
        }
        catch (Exception exception) when (
            exception is FormatException or
                JsonException or
                ArgumentOutOfRangeException)
        {
            return Anonymous;
        }
    }

    private static void AddClaim(
        JsonElement payload,
        string propertyName,
        string claimType,
        ICollection<Claim> claims)
    {
        if (!payload.TryGetProperty(propertyName, out var value) ||
            value.ValueKind != JsonValueKind.String)
        {
            return;
        }

        var claimValue = value.GetString();

        if (!string.IsNullOrWhiteSpace(claimValue))
        {
            claims.Add(new Claim(claimType, claimValue));
        }
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var base64 = value
            .Replace('-', '+')
            .Replace('_', '/');

        var padding = (base64.Length % 4) switch
        {
            0 => string.Empty,
            2 => "==",
            3 => "=",
            _ => throw new FormatException("Invalid Base64URL payload.")
        };

        base64 += padding;

        return Convert.FromBase64String(base64);
    }
}