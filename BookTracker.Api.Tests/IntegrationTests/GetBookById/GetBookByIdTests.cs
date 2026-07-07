using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.GetBookById;

public class GetBookByIdTests : IntegrationTest
{


    [Fact]
    public async Task GetBookByIdReturnsBook()
    {
        var writer = Writer;

        writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });


        var response = await Client.GetAsync("/books/1");

        var book = await response.ReadJsonAs<BookDetails>(HttpStatusCode.OK);

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

        var response = await Client.GetAsync("/books/9999");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}