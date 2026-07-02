using System.Net;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.DeleteBook;

public class DeleteBookTests
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task DeleteBookRemovesBook()
    {
        var writer = factory.GetWriter();

        writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Id = 1,
                    Title = "Dune",
                    Author = "Frank Herbert",
                    Year = 1965
                });
        });

        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/books/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var reader = factory.GetReader();
        var book = reader.Query(db => db.Books.Find(1));

        Assert.Null(book);
    }

    [Fact]
    public async Task DeleteBookReturnsNotFoundWhenBookDoesNotExist()
    {
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/books/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}