using Microsoft.JSInterop;

namespace BookTracker.Blazor.Auth;

public sealed class AuthSession(IJSRuntime jsRuntime) : IAuthSession
{
    private const string AccessTokenKey = "bookTracker.accessToken";

    public async Task<string?> GetTokenAsync() =>
        await jsRuntime.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);

    public async Task SetTokenAsync(string token) =>
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, token);

    public async Task ClearTokenAsync() =>
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
}