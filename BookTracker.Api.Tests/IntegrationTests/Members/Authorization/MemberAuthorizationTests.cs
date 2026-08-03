using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Application.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.Authorization;

[Collection(PostgreSqlCollection.Name)]
public class MemberAuthorizationTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task CreateMemberDoesNotRequireAuthentication()
    {
        var request = new CreateMemberRequest
        {
            Name = "Grace Hopper",
            Email = "grace@example.com",
            Password = "debugging-moth"
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateMemberRequiresAuthentication()
    {
        var memberId = SeedMember("Test User", "test@example.com");

        var request = new UpdateMemberRequest
        {
            Name = "Changed",
            Email = "changed@example.com"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MemberCanUpdateOwnAccount()
    {
        var memberId = await AuthenticateAsMember();

        var request = new UpdateMemberRequest
        {
            Name = "Ada Byron",
            Email = "ada.byron@example.com"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task MemberCannotUpdateAnotherMember()
    {
        await AuthenticateAsMember();

        var otherMemberId = SeedMember("Grace Hopper", "grace@example.com");

        var request = new UpdateMemberRequest
        {
            Name = "Hacked Name",
            Email = "hacked@example.com"
        };

        var response = await Client.PutAsJsonAsync($"/members/{otherMemberId}", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task MemberListRequiresAuthentication()
    {
        var response =
            await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task RegularMemberCannotViewMemberList()
    {
        await AuthenticateAsMember();

        var response =
            await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdministratorCanViewMemberList()
    {
        await AuthenticateAsMember(
            MemberRole.Administrator);

        var response =
            await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.OK);
    }
    [Fact]
    public async Task MemberDetailsRequiresAuthentication()
    {
        var response = await Client.GetAsync("/members/1");
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegularMemberCannotViewOtherMemberDetails()
    {
        await AuthenticateAsMember();

        var otherMemberId = SeedMember("Grace Hopper", "grace@example.com");

        var response = await Client.GetAsync($"/members/{otherMemberId}");
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdministratorCanViewAnyMemberDetails()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var otherMemberId = SeedMember("Grace Hopper", "grace@example.com");

        var response = await Client.GetAsync($"/members/{otherMemberId}");
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }
}
