using System.ComponentModel.DataAnnotations;

namespace BookTracker.Blazor.Models.Auth;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public required string AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}