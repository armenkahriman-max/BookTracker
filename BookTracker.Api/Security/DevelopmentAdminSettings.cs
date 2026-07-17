namespace BookTracker.Api.Security;

public class DevelopmentAdminSettings
{
    public const string SectionName =
        "DevelopmentAdmin";

    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}