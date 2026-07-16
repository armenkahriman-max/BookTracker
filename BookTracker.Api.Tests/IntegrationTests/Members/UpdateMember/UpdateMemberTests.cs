using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.UpdateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;


namespace BookTracker.Api.Tests.IntegrationTests.UpdateMember;

public class UpdateMember : IntegrationTest
{

    [Fact]
    public async Task PutMemberUpdatesMember()
    {
        var writer = Writer;

        writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Donner"),
                    Email = new MemberEmail("Chugns@gmail.com"),
                    PasswordHash = "test-password-hash"
                });
        });

        var request =
            new UpdateMemberRequest
            {
                Name = "Dean",
                Email = "Chugns@gmail.com",


            };



        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var reader = Reader;
        var member = reader.Query(db => db.Members.Find(1));

        Assert.NotNull(member);
        Assert.Equal("Dean", member.Name.Value);
        Assert.Equal("chugns@gmail.com", member.Email.Value);

    }

    [Fact]
    public async Task PutMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        var request =
            new UpdateMemberRequest
            {
                Name = "Unknown Member",
                Email = "unknown@gmail.com",

            };



        var response = await Client.PutAsJsonAsync("/members/9999", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]

    public async Task PutMemberReturnsBadRequestWhenEmailIsInvalid()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(new Member
            {
                Name = new MemberName("John Doe"),
                Email = new MemberEmail("john@example.com"),
                PasswordHash = "test-password-hash"
            });
        });

        var request = new UpdateMemberRequest
        {
            Name = "John Doe",
            Email = "invalid-email"
        };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task PutMemberReturnsConflictWhenEmailAlreadyExists()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Id = 1,
                    Name = new MemberName("Ada Lovelace"),
                    Email = new MemberEmail("ada@example.com"),
                    PasswordHash = "test-password-hash"
                },
                new Member
                {
                    Id = 2,
                    Name = new MemberName("Bob Smith"),
                    Email = new MemberEmail("bob@example.com"),
                    PasswordHash = "test-password-hash"
                });
        });

        var request = new UpdateMemberRequest
        {
            Name = "Ada Lovelace",
            Email = "bob@example.com"
        };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PutMemberAllowsKeepingOwnEmail()
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

        var request = new UpdateMemberRequest
        {
            Name = "Ada Updated",
            Email = "ADA@EXAMPLE.COM"
        };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }
}