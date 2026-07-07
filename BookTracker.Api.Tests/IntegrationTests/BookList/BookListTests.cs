using System.Net.Http.Json;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application;
using BookTracker.Api.Domain;


namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests : IntegrationTest
{

    [Fact]
    public async Task GetBooksReturnsBooks()
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
        var result = await Client.GetFromJsonAsync<PagedResult<BookInfo>>("/books");

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
    public async Task GetBooksReturnsRequestedPage()
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

        var result = await Client.GetFromJsonAsync<PagedResult<BookInfo>>("/books?page=2&pageSize=1");

        Assert.NotNull(result);

        var book = Assert.Single(result.Items);

        Assert.Equal("Book 2", book.Title);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
    }


    [Fact]
    public async Task GetBooksReturnsEmptyItemsWhenPageIsTooHigh()
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

        var result = await Client.GetFromJsonAsync<PagedResult<BookInfo>>("/books?page=99&pageSize=10");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(99, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }
}