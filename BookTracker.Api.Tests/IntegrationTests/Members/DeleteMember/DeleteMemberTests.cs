using System.Net;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.DeleteMember;

public class DeleteMember : IntegrationTest
{

    [Fact]
    public async Task DeleteMemberRemovesMember()
    {
        var writer = Writer;

        writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Id = 1,
                    Name = new MemberName("Chung Lee"),
                    Email = new MemberEmail("Chung@gmail.com"),
                    PasswordHash = "test-password-hash"
                });


        });
        var response = await Client.DeleteAsync("/members/1");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.Null(member);
    }

    [Fact]
    public async Task DeleteMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        var response = await Client.DeleteAsync("/members/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMemberReturnsNotFoundAfterDeletingMember()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(new Member
            {
                Id = 1,
                Name = new MemberName("Ada Lovelace"),
                Email = new MemberEmail("ada@example.com"),
                PasswordHash = "test-password-hash"
            });
        });

        var deleteResponse = await Client.DeleteAsync("/members/1");

        await deleteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync("/members/1");

        await getResponse.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}