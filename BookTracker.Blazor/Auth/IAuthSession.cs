namespace BookTracker.Blazor.Auth;

public interface IAuthSession
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task ClearTokenAsync();
}