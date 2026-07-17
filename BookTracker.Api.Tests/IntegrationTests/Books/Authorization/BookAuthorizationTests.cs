using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;

namespace BookTracker.Api.Tests.IntegrationTests.Books.Authorization;

public class BookAuthorizationTests : IntegrationTest
{
    [Fact]
    public async Task CreateBookRequiresAuthentication()
    {
        var request = new CreateBookRequest
        {
            Title = "Dune",
            Author = "Frank Herbert",
            Year = 1965
        };

        var response = await Client.PostAsJsonAsync("/books", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

        // Make sure no book was created
        var count = Reader.Query(db => db.Books.Count());
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetBooksDoesNotRequireAuthentication()
    {
        var response = await Client.GetAsync("/books");
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }
}