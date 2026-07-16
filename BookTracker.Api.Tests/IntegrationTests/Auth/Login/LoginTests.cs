using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Auth.Login;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Auth.Login;

public class LoginTests : IntegrationTest
{
    private void SeedMember(string password = "analytical-engine")
    {
        var member = new Member
        {
            Name = new MemberName("Ada Lovelace"),
            Email = new MemberEmail("ada@example.com"),
            PasswordHash = string.Empty
        };

        var passwordHasher = new PasswordHasher<Member>();
        member.PasswordHash = passwordHasher.HashPassword(member, password);

        Writer.Seed(db => db.Members.Add(member));
    }

    [Fact]
    public async Task LoginReturnsAccessToken()
    {
        SeedMember();

        var request = new LoginRequest
        {
            Email = "ada@example.com",
            Password = "analytical-engine"
        };

        var response = await Client.PostAsJsonAsync("/auth/login", request);

        var login = await response.ReadJsonAs<LoginResponse>(HttpStatusCode.OK);

        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.True(login.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginNormalizesEmail()
    {
        SeedMember();

        var request = new LoginRequest
        {
            Email = "  ADA@EXAMPLE.COM  ",
            Password = "analytical-engine"
        };

        var response = await Client.PostAsJsonAsync("/auth/login", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoginReturnsUnauthorizedForWrongPassword()
    {
        SeedMember();

        var request = new LoginRequest
        {
            Email = "ada@example.com",
            Password = "wrong-password"
        };

        var response = await Client.PostAsJsonAsync("/auth/login", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginReturnsUnauthorizedForUnknownEmail()
    {
        SeedMember();

        var request = new LoginRequest
        {
            Email = "unknown@example.com",
            Password = "analytical-engine"
        };

        var response = await Client.PostAsJsonAsync("/auth/login", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }
}