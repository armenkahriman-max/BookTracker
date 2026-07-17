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
        var memberId = await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            var newMember = new Member
            {
                Name = new MemberName("Chung"),
                Email = new MemberEmail("chung@gmail.com"),
                PasswordHash = "test-password-hash",
                Role = MemberRole.Member
            };

            db.Members.Add(newMember);
        });


        var member = Reader.Query(db => db.Members.OrderByDescending(m => m.Id).First());

        var response = await Client.GetAsync($"/members/{member.Id}");

        var result = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(member.Id, result.Id);
        Assert.Equal("Chung", result.Name);
        Assert.Equal("chung@gmail.com", result.Email);
    }

    [Fact]
    public async Task GetMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/members/999999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}