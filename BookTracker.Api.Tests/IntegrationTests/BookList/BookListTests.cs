using System.Net.Http.Json;
using BookTracker.Api.Application.BookList;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;


namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests : IntegrationTest
{

    [Fact]
    public async Task GetBooksReturnsBooks()
    {
        var response = await Client.GetAsync("/books");
        var books = await response.Content.ReadFromJsonAsync<List<BookInfo>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(books);
        Assert.Empty(books);
    }
}