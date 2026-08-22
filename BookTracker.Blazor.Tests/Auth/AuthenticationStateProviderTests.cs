using System.Text;
using System.Text.Json;
using BookTracker.Blazor.Auth;

namespace BookTracker.Blazor.Tests.Auth;

public class AuthenticationStateProviderTests
{
    private static string CreateJwt(object payload)
    {
        var header = Base64Url("{\"alg\":\"none\",\"typ\":\"JWT\"}");
        var body = Base64Url(JsonSerializer.Serialize(payload));
        return $"{header}.{body}.sig";
    }

    private static string Base64Url(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static BookTrackerAuthenticationStateProvider CreateProvider(string? token)
    {
        return new BookTrackerAuthenticationStateProvider(new FakeAuthSession(token));
    }

    [Fact]
    public async Task Valid_Token_Gives_Authenticated_User()
    {
        var token = CreateJwt(new Dictionary<string, object>
        {
            ["nameid"] = "1",
            ["unique_name"] = "Ada Reader",
            ["email"] = "ada@example.com",
            ["role"] = "Administrator",
            ["exp"] = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
        });

        var provider = CreateProvider(token);
        var state = await provider.GetAuthenticationStateAsync();

        Assert.True(state.User.Identity!.IsAuthenticated);
        Assert.Equal("Ada Reader", state.User.Identity.Name);
        Assert.True(state.User.IsInRole("Administrator"));
    }

    [Fact]
    public async Task Expired_Token_Gives_Anonymous_User()
    {
        var token = CreateJwt(new Dictionary<string, object>
        {
            ["nameid"] = "1",
            ["unique_name"] = "Ada Reader",
            ["email"] = "ada@example.com",
            ["role"] = "Administrator",
            ["exp"] = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds()
        });

        var provider = CreateProvider(token);
        var state = await provider.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity!.IsAuthenticated);
    }

    [Fact]
    public async Task Unreadable_Token_Gives_Anonymous_User()
    {
        var badTokens = new[]
        {
            "",
            "not.a.jwt",
            "only.two.parts",
            "header.badbase64!!.sig"
        };

        foreach (var token in badTokens)
        {
            var provider = CreateProvider(token);
            var state = await provider.GetAuthenticationStateAsync();
            Assert.False(state.User.Identity!.IsAuthenticated);
        }
    }

    private sealed class FakeAuthSession(string? token) : IAuthSession
    {
        public Task<string?> GetTokenAsync() => Task.FromResult(token);
        public Task SetTokenAsync(string token) => Task.CompletedTask;
        public Task ClearTokenAsync() => Task.CompletedTask;
    }
}