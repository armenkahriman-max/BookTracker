using System.Net;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.DeleteMember;

public class DeleteMember : IntegrationTest
{
    [Fact]
    public async Task DeleteMemberRemovesMember()
    {
        var memberId = await AuthenticateAsMember();

        var response = await Client.DeleteAsync($"/members/{memberId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var member = Reader.Query(db => db.Members.Find(memberId));
        Assert.Null(member);
    }

    [Fact]
    public async Task DeleteMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        await AuthenticateAsMember();

        var response = await Client.DeleteAsync("/members/999999");
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteMemberReturnsNotFoundAfterDeletingMember()
    {
        var memberId = await AuthenticateAsMember();

        var deleteResponse = await Client.DeleteAsync($"/members/{memberId}");
        await deleteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var response = await Client.DeleteAsync($"/members/{memberId + 99999}");
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }
}