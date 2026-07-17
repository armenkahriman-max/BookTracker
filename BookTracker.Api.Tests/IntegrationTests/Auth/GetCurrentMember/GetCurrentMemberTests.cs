using System.Net;
using System.Net.Http.Headers;
using BookTracker.Api.Application.Auth.GetCurrentMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Auth.GetCurrentMember;

public class GetCurrentMemberTests : IntegrationTest
{
    [Fact]
    public async Task GetCurrentMemberRequiresAuthentication()
    {
        var response = await Client.GetAsync("/auth/me");
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentMemberReturnsTokenClaims()
    {
        var memberId = await AuthenticateAsMember(
            MemberRole.Member, 
            "Ada Lovelace", 
            "ada@example.com");

        var response = await Client.GetAsync("/auth/me");
        var member = await response.ReadJsonAs<CurrentMemberResponse>(HttpStatusCode.OK);

        Assert.Equal(memberId, member.Id);
        Assert.Equal("Ada Lovelace", member.Name);
        Assert.Equal("ada@example.com", member.Email);
        Assert.Equal("Member", member.Role);
    }

    [Fact]
    public async Task GetCurrentMemberReturnsRole()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/auth/me");

        var member = await response.ReadJsonAs<CurrentMemberResponse>(HttpStatusCode.OK);

        Assert.Equal("Administrator", member.Role);
    }

    [Fact]
    public async Task GetCurrentMemberRejectsInvalidToken()
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "this-is-not-a-valid-token");

        var response = await Client.GetAsync("/auth/me");
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }
}