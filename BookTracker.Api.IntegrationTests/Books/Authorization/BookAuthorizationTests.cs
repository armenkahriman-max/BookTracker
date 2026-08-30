using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;

namespace BookTracker.Api.IntegrationTests.IntegrationTests.Books.Authorization;

[Collection(PostgreSqlCollection.Name)]
public class BookAuthorizationTests(PostgreSqlFixture database) : IntegrationTest(database)
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


        var count = Reader.Query(db => db.Books.Count());
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetBooksDoesNotRequireAuthentication()
    {
        var response = await Client.GetAsync("/books");
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegularMemberCannotCreateBook()
    {
        await AuthenticateAsMember();

        var request =
            new CreateBookRequest
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Year = 1965
            };

        var response =
            await Client.PostAsJsonAsync(
                "/books",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var count =
            Reader.Query(db =>
                db.Books.Count());

        Assert.Equal(0, count);
    }
}