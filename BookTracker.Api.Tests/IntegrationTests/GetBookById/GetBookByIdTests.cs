using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.GetBookById;

public class GetBookByIdTests
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetBookByIdReturnsBook()
    {
        var writer = factory.GetWriter();

        writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = "Dune",
                    Author = "Frank Herbert",
                    Year = 1965
                });
        });

        var client = factory.CreateClient();

        var response = await client.GetAsync("/books/1");
        var book = await response.Content.ReadFromJsonAsync<BookDetails>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(book);
        Assert.Equal(1, book.Id);
        Assert.Equal("Dune", book.Title);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal(1965, book.Year);
    }

    [Fact]
    public async Task GetBookByIdReturnsNotFoundWhenBookDoesNotExist()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/books/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}