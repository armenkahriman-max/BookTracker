namespace BookTracker.Api.Security;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required string SigningKey { get; set; }

    public int ExpirationMinutes { get; set; } = 60;
}