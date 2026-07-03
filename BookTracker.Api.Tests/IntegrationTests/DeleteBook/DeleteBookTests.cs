using System.Net;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.DeleteBook;

public class DeleteBookTests : IntegrationTest
{
    
    [Fact]
    public async Task DeleteBookRemovesBook()
    {
        var writer = Writer;

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

       

        var response = await Client.DeleteAsync("/books/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        
        var book = Reader.Query(db => db.Books.Find(1));

        Assert.Null(book);
    }

    [Fact]
    public async Task DeleteBookReturnsNotFoundWhenBookDoesNotExist()
    {

        var response = await Client.DeleteAsync("/books/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}