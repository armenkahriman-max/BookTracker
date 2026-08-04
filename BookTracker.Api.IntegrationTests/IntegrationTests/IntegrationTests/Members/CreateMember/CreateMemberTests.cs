using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.IntegrationTests.IntegrationTests.CreateMember;

[Collection(PostgreSqlCollection.Name)]
public class CreateMemberTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task PostMemberCreatesMember()
    {
        // No authentication needed for registration
        var request = new CreateMemberRequest
        {
            Name = "Ada Lovelace",
            Email = "ada@example.com",
            Password = "analytical-engine"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Ada Lovelace", created.Name);

        var member = Reader.Query(context => context.Find<Member>(created.Id));

        Assert.NotNull(member);
        Assert.NotEqual("analytical-engine", member.PasswordHash);

        var passwordHasher = new PasswordHasher<Member>();
        var result = passwordHasher.VerifyHashedPassword(
            member,
            member.PasswordHash,
            "analytical-engine");

        Assert.Equal(PasswordVerificationResult.Success, result);

        Assert.Equal("Ada Lovelace", member.Name.Value);
        Assert.Equal("ada@example.com", member.Email.Value);
        Assert.Equal(MemberRole.Member, member.Role);   // Extra check
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenNameOrEmailIsInvalid()
    {
        var request = new CreateMemberRequest
        {
            Name = "  ",
            Email = "adaexample.com",
            Password = "analytical-engine"
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenPasswordIsEmpty()
    {
        var request = new CreateMemberRequest
        {
            Name = "Ada Lovelace",
            Email = "ada@example.com",
            Password = ""
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenPasswordIsTooShort()
    {
        var request = new CreateMemberRequest
        {
            Name = "Ada Lovelace",
            Email = "ada@example.com",
            Password = "1234567"
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMemberReturnsConflictWhenEmailAlreadyExists()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(new Member
            {
                Name = new MemberName("Ada"),
                Email = new MemberEmail("ada@example.com"),
                PasswordHash = "test-password-hash"
            });
        });

        var request = new CreateMemberRequest
        {
            Name = "Another Ada",
            Email = "ADA@EXAMPLE.COM",
            Password = "analytical-engine"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateMemberCreatesRegularMember()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Grace Hopper",
                Email = "grace@example.com",
                Password = "debugging-moth"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/members",
                request);

        var created =
            await response
                .ReadJsonAs<CreateMemberResponse>(
                    HttpStatusCode.Created);

        var member =
            Reader.Query(db =>
                db.Members.Find(created.Id));

        Assert.NotNull(member);

        Assert.Equal(
            MemberRole.Member,
            member.Role);
    }
}