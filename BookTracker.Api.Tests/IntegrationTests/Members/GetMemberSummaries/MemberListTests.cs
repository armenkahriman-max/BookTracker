using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.MemberList;

[Collection(PostgreSqlCollection.Name)]
public class GetMemberListTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task GetMemberSummariesReturnMemberSummaries()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        // Use a unique name to avoid conflicts with previous seeds
        var uniqueName = "UniqueTestMember" + Guid.NewGuid().ToString()[..8];

        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName(uniqueName),
                    Email = new MemberEmail("unique@test.com"),
                    PasswordHash = "test-password-hash"
                });
        });

        var response = await Client.GetAsync($"/members?search={uniqueName}");
        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        Assert.NotNull(result);
        var memberInfo = Assert.Single(result.Items);
        Assert.Equal(uniqueName, memberInfo.Name);
        Assert.Equal("unique@test.com", memberInfo.Email);

        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsNotFoundWhenSummeriesDoesNotExist()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/members/999999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMembersSearchesByName()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Members.Add(new Member
            {
                Name = new MemberName("ChungySearch"),
                Email = new MemberEmail("chungysearch@example.com"),
                PasswordHash = "test-password-hash"
            });
        });

        var response = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>(
            "/members?search=ChungySearch");

        var member = Assert.Single(response!.Items);
        Assert.Equal("ChungySearch", member.Name);
    }


    [Fact]
    public async Task GetMembersSummariesCanSearchesByEmail()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Mark"),
                    Email = new MemberEmail("marksearch@example.com"),
                    PasswordHash = "test-password-hash"
                });
        });

        var response = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>(
            "/members?search=marksearch@example.com");

        var member = Assert.Single(response!.Items);
        Assert.Equal("Mark", member.Name);
    }

    [Fact]
    public async Task GetMembersSummariesAppliesPagingAfterSearch()
    {
        await AuthenticateAsMember(MemberRole.Administrator);


        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member { Name = new MemberName("Joe One"), Email = new MemberEmail("joe1@gmail.com"), PasswordHash = "test" },
                new Member { Name = new MemberName("Joe Two"), Email = new MemberEmail("joe2@gmail.com"), PasswordHash = "test" },
                new Member { Name = new MemberName("Frank"), Email = new MemberEmail("frank@gmail.com"), PasswordHash = "test" });
        });

        var response = await Client.GetAsync("/members?search=Joe&page=2&pageSize=1");
        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);
        Assert.Equal("Joe Two", member.Name);
    }
}