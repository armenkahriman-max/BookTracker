using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.UpdateBook;

[Collection(PostgreSqlCollection.Name)]
public class UpdateBookTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task PutBookUpdatesBook()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Books.Add(new Book
            {
                Title = new BookTitle("Dune"),
                Author = new AuthorName("Frank Herbert"),
                Year = 1965
            });
        });


        var getResponse = await Client.GetAsync("/books/1");
        var currentBook = await getResponse.ReadJsonAs<GetBookDetailsResponse>(HttpStatusCode.OK);


        var request = new UpdateBookRequest
        {
            Title = "Dune Messiah",
            Author = "Frank Herbert",
            Year = 1969,
            Version = currentBook.Version
        };

        var response = await Client.PutAsJsonAsync("/books/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify
        var updatedBook = Reader.Query(db => db.Books.Find(1));
        Assert.NotNull(updatedBook);
        Assert.Equal("Dune Messiah", updatedBook.Title.Value);
        Assert.Equal("Frank Herbert", updatedBook.Author.Value);
        Assert.Equal(1969, updatedBook.Year);
        Assert.NotEqual(currentBook.Version, updatedBook.Version);
    }

    [Fact]
    public async Task PutBookReturnsNotFoundWhenBookDoesNotExist()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var request = new UpdateBookRequest
        {
            Title = "Unknown Book",
            Author = "Unknown Author",
            Year = 2000,
            Version = Guid.NewGuid()
        };

        var response = await Client.PutAsJsonAsync("/books/9999", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutBookReturnsConflictForStaleVersion()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Books.Add(new Book
            {
                Title = new BookTitle("Dune"),
                Author = new AuthorName("Frank Herbert"),
                Year = 1965
            });
        });


        var firstGet = await Client.GetAsync("/books/1");
        var firstRead = await firstGet.ReadJsonAs<GetBookDetailsResponse>(HttpStatusCode.OK);


        var secondGet = await Client.GetAsync("/books/1");
        var secondRead = await secondGet.ReadJsonAs<GetBookDetailsResponse>(HttpStatusCode.OK);


        var firstUpdate = new UpdateBookRequest
        {
            Title = "Dune: Special Edition",
            Author = firstRead.Author,
            Year = firstRead.Year,
            Version = firstRead.Version
        };

        var firstResponse = await Client.PutAsJsonAsync("/books/1", firstUpdate);
        await firstResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);


        var staleUpdate = new UpdateBookRequest
        {
            Title = secondRead.Title,
            Author = secondRead.Author,
            Year = 1966,
            Version = secondRead.Version
        };

        var staleResponse = await Client.PutAsJsonAsync("/books/1", staleUpdate);
        await staleResponse.ShouldHaveStatusCode(HttpStatusCode.Conflict);


        var finalBook = Reader.Query(db => db.Books.Find(1));
        Assert.Equal("Dune: Special Edition", finalBook.Title.Value);
        Assert.Equal(1965, finalBook.Year);
    }
}