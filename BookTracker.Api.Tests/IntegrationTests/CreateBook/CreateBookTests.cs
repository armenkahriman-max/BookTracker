using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.CreateBook;

public class CreateBookTests
{
    private readonly WebApplicationFactory<Program> factory = new();

    [Fact]
    public async Task PostBookCreatesBook()
    {
        var request =
            new CreateBookRequest
            {
                Title = "The Heart Is a Lonely Hunter",
                Author = "Carson McCullers",
                Year = 1940
            };
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/books", request);
        var created = await response.Content.ReadFromJsonAsync<CreateBookResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("The Heart Is a Lonely Hunter", created.Title);
    }
}