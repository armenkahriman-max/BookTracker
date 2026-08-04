using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.IntegrationTests.IntegrationTests.UpdateMember;

[Collection(PostgreSqlCollection.Name)]
public class UpdateMemberTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task PutMemberUpdatesMember()
    {
        var memberId = await AuthenticateAsMember();

        var request = new UpdateMemberRequest
        {
            Name = "Dean",
            Email = "dean@example.com"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var member = Reader.Query(db => db.Members.Find(memberId));
        Assert.NotNull(member);
        Assert.Equal("Dean", member.Name.Value);
        Assert.Equal("dean@example.com", member.Email.Value);
    }

    [Fact]
    public async Task PutMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        await AuthenticateAsMember();

        var request = new UpdateMemberRequest
        {
            Name = "Unknown",
            Email = "unknown@example.com"
        };

        var response = await Client.PutAsJsonAsync("/members/999999", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutMemberReturnsBadRequestWhenEmailIsInvalid()
    {
        var memberId = await AuthenticateAsMember();

        var request = new UpdateMemberRequest
        {
            Name = "John Doe",
            Email = "invalid-email"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutMemberReturnsConflictWhenEmailAlreadyExists()
    {
        var memberId = await AuthenticateAsMember();


        SeedMember("Bob Smith", "bob@example.com");

        var request = new UpdateMemberRequest
        {
            Name = "Ada",
            Email = "bob@example.com"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PutMemberAllowsKeepingOwnEmail()
    {
        var memberId = await AuthenticateAsMember(
            MemberRole.Member,
            "Ada Lovelace",
            "ada@example.com");

        var request = new UpdateMemberRequest
        {
            Name = "Ada Updated",
            Email = "ADA@EXAMPLE.COM"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }
}