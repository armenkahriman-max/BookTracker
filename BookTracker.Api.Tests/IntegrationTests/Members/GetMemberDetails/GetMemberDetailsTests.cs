using System.Net;
using BookTracker.Api.Application.Members.GetMemberDetailsResponse;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.GetMemberDetailsTests;

public class GetMemberDetailsTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberDetailsReturnsMemberDetails()
    {
        var writer = Writer;
        writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Chung"),
                    Email = new MemberEmail("chung@gmail.com"),
                    PasswordHash = "test-password-hash"
                });
        });
        var response = await Client.GetAsync("/members/1");

        var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(member);
        Assert.Equal(1, member.Id);
        Assert.Equal("Chung", member.Name);
        Assert.Equal("chung@gmail.com", member.Email);
    }

    [Fact]
    public async Task GetMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        var response = await Client.GetAsync("/members/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    }
}