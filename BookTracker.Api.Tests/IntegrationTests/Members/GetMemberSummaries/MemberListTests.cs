using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.MemberList;

public class MemberListTest : IntegrationTest
{
    [Fact]
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

    [Fact]
    public async Task GetMembersSearchesByName()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(new Member
            {
                Name = new MemberName("Chungy"),
                Email = new MemberEmail("Chungy@example.com")
            });

            db.Members.Add(new Member
            {
                Name = new MemberName("Doe"),
                Email = new MemberEmail("Doe@example.com")
            });
        });

        var response = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>(
            "/members?search=Chungy");

        var member = Assert.Single(response!.Items);
        Assert.Equal("Chungy", member.Name);
    }

    [Fact]
    public async Task GetMembersSummariesCanSearchesByEmail()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Mark"),
                    Email = new MemberEmail("Mark@example.com")
                });

            db.Members.Add(new Member
            {
                Name = new MemberName("Jane"),
                Email = new MemberEmail("jane@example.com")
            });
        });

        var response = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>(
            "/members?search=jane@example.com");

        var member = Assert.Single(response!.Items);
        Assert.Equal("Jane", member.Name);
    }

    [Fact]
    public async Task GetMembersSummariesAppliesPagingAfterSearch()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Joe One"),
                    Email = new MemberEmail("joe1@gmail.com")
                },
                new Member
                {
                    Name = new MemberName("Joe Two"),
                    Email = new MemberEmail("joe2@gmail.com")
                },
                new Member
                {
                    Name = new MemberName("Frank"),
                    Email = new MemberEmail("frank@gmail.com")
                });
        });

        var response = await Client.GetAsync("/members?search=Joe&page=2&pageSize=1");
        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Joe Two", member.Name);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }


}