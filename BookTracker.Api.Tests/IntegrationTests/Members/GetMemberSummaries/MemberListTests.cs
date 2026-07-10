using System.Net;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.MemberList;

public class MemberListTest : IntegrationTest
{

    public async Task GetMemberSummariesReturnMemberSummaries()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Chungy"),
                    Email = new MemberEmail("chungy@gmail.com")
                });

        });
        var response = await Client.GetAsync("/members");
        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        Assert.NotNull(result);
        var memberInfo = Assert.Single(result.Items);
        Assert.Equal("Chungy", memberInfo.Name);
        Assert.Equal("chungy@gmail.com", memberInfo.Email);

        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsNotFoundWhenSummeriesDoesNotExist()
    {
        var response = await Client.GetAsync("/members/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}