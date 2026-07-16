namespace BookTracker.Api.Application.Auth.Login;

public class LoginResponse
{
    public required string AccessToken { get; set; }

    public DateTime ExpiresAt { get; set; }
}