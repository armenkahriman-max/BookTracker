using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Domain;
using System.Net;
using BookTracker.Api.Application.GetBookSummaries;
using BookTracker.Api.Domain.Books;


namespace BookTracker.Api.Tests.IntegrationTests.BookList;

[Collection(PostgreSqlCollection.Name)]
public class GetBookListTests(PostgreSqlFixture database) : IntegrationTest(database)
{

    [Fact]
    public async Task GetBookSummariesReturnsBookSummaries()
    {
        Writer.Seed(db =>
{
    db.Books.Add(
        new Book
        {
            Title = new BookTitle("Cannery Row"),
            Author = new AuthorName("John Steinbeck"),
            Year = 1945
        });
});
        var response = await Client.GetAsync("/books");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        Assert.NotNull(result);

        var bookInfo = Assert.Single(result.Items);

        Assert.Equal("Cannery Row", bookInfo.Title);
        Assert.Equal("John Steinbeck", bookInfo.Author);

        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetBookSummariesReturnsRequestedPage()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Book 1"),
                    Author = new AuthorName("Author 1"),
                    Year = 2001
                },
                new Book
                {
                    Title = new BookTitle("Book 2"),
                    Author = new AuthorName("Author 2"),
                    Year = 2002
                },
                new Book
                {
                    Title = new BookTitle("Book 3"),
                    Author = new AuthorName("Author 3"),
                    Year = 2003
                });
        });

        var result = await Client.GetFromJsonAsync<PagedResult<BookSummary>>("/books?page=2&pageSize=1");

        Assert.NotNull(result);

        var book = Assert.Single(result.Items);

        Assert.Equal("Book 2", book.Title);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
    }


    [Fact]
    public async Task GetBookSummariesReturnsEmptyItemsWhenPageIsTooHigh()
    {
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Book 1"),
                    Author = new AuthorName("Author 1"),
                    Year = 2001
                });
        });

        var result = await Client.GetFromJsonAsync<PagedResult<BookSummary>>("/books?page=99&pageSize=10");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(99, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetBooksSummariesCanSearchByTitle()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                },
                new Book
                {
                    Title = new BookTitle("The Big Sleep"),
                    Author = new AuthorName("Raymond Chandler"),
                    Year = 1939
                });
        });

        var response = await Client.GetAsync("/books?search=dune");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        var book = Assert.Single(result.Items);

        Assert.Equal("Dune", book.Title);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetBookSummariesAppliesPagingAfterSearch()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                },
                new Book
                {
                    Title = new BookTitle("Dune Messiah"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1969
                },
                new Book
                {
                    Title = new BookTitle("The Big Sleep"),
                    Author = new AuthorName("Raymond Chandler"),
                    Year = 1939
                });
        });

        var response = await Client.GetAsync("/books?search=dune&page=2&pageSize=1");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        var book = Assert.Single(result.Items);

        Assert.Equal("Dune Messiah", book.Title);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetBookSummaries_ReturnsEmptyList_WhenNoBookExists()
    {
        var response = await Client.GetAsync("/books");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetBookSummaries_ReturnsEmptyList_WhenSearchHasNoMatch()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                },
                new Book
                {
                    Title = new BookTitle("The Big Sleep"),
                    Author = new AuthorName("Raymond Chandler"),
                    Year = 1939
                });
        });
        var response = await Client.GetAsync("/books?search=nonexistentbooktitlexyz123");
        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }
}
