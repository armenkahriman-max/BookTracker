using System.Net;
using System.Text;
using System.Text.Json;
using BookTracker.Blazor.Api;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using BookTracker.Blazor.Pages.Books.Edit;
using BookTracker.Blazor.Models.Books.Update;
using BookTracker.Blazor.Models.Books;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AngleSharp.Io;
using Microsoft.Extensions.Caching.Memory;
namespace BookTracker.Blazor.Tests.Components.Books;

public class UpdateBookTests : BunitContext
{
    private void RegisterClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new BookTrackerClient(httpClient));

    }

    private static HttpResponseMessage Json(
        object body,
        HttpStatusCode code = HttpStatusCode.OK)
    {
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return new HttpResponseMessage(code)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }


    [Fact]
    public void HasAdministratorAuthorizeAttribute()
    {
        var attribute = typeof(EditBook)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(attribute);
        Assert.Equal("Administrator", attribute.Roles);
    }


    [Fact]
    public void ExistingDataIsFilledInTheForm()
    {
        var existingBook = new BookDetailsResponse()
        {

            Title = "Testing Title",
            Author = "Testing Author",
            Year = 2024,
            Version = Guid.NewGuid()
        };
        var handler = new FakeHandler(_ =>
        Task.FromResult(Json(existingBook, HttpStatusCode.OK)));

        RegisterClient(handler);

        var cut = Render<EditBook>(parameters => parameters
        .Add(p => p.id, 1));

        var input = cut.FindAll("input");
        Assert.Equal("Testing Title", input[0].GetAttribute("value"));
        Assert.Equal("Testing Author", input[1].GetAttribute("value"));
        Assert.Equal("2024", input[2].GetAttribute("value"));
    }

    [Fact]
    public async Task StatusCode204SivesASuccessfulFlow()
    {
        var existingBook = new BookDetailsResponse()
        {
            Title = "Test Title",
            Author = "Test Author",
            Year = 2025,
            Version = Guid.NewGuid()
        };
        var handler = new FakeHandler(request =>
        {
            if (request.Method.Method == "GET")
                return Task.FromResult(Json(existingBook, HttpStatusCode.OK));

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
        });
        RegisterClient(handler);

        var cut = Render<EditBook>(parameters => parameters.Add(p => p.id, 1));

        await cut.InvokeAsync(async () =>
        {
            await cut.FindAll("input")[0].ChangeAsync("Test Title");
            await cut.FindAll("input")[1].ChangeAsync("Test Author");
            await cut.FindAll("input")[2].ChangeAsync("2025");

            await cut.Find("form").SubmitAsync();
        });

        var navMan = Services.GetRequiredService<NavigationManager>();
        Assert.Equal("http://localhost/books/1", navMan.Uri);

    }
    [Fact]
    public void StatusCode404IsShown()
    {
        var handler = new FakeHandler(_ =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));
        RegisterClient(handler);

        var cut = Render<EditBook>(parameters => parameters.Add(p => p.id, 1));
        Assert.Contains("Book not found.", cut.Markup);
    }
    [Fact]
    public async Task StatusCode409ShowsTheConflictState()
    {
        var existingBook = new BookDetailsResponse()
        {
            Title = "Test Title",
            Author = "Test Author",
            Year = 2025,
            Version = Guid.NewGuid()
        };

        var handler = new FakeHandler(request =>
        {
            if (request.Method.Method == "GET")
                return Task.FromResult(Json(existingBook, HttpStatusCode.OK));

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict));
        });

        RegisterClient(handler);

        var cut = Render<EditBook>(parameters => parameters.Add(p => p.id, 1));

        await cut.InvokeAsync(async () =>
        {
            await cut.FindAll("input")[0].ChangeAsync("Test Title");
            await cut.FindAll("input")[1].ChangeAsync("Test Author");
            await cut.FindAll("input")[2].ChangeAsync("2025");

            await cut.Find("form").SubmitAsync();
        });

        Assert.Contains("The book was changed by another user.", cut.Markup);
        Assert.Contains("Reload latest data", cut.Markup);

    }
    [Fact]
    public async Task ReloadFetchesTheNewVersion()
    {
        var versionA = Guid.NewGuid();
        var versionB = Guid.NewGuid();
        int getCount = 0;
        HttpRequestMessage? lastPut = null;

        var handler = new FakeHandler(request =>
        {
            if (request.Method.Method == "GET")
            {
                getCount++;
                var book = new BookDetailsResponse
                {
                    Title = getCount == 1 ? "Old Title" : "New Title",
                    Author = "Author",
                    Year = 2024,
                    Version = getCount == 1 ? versionA : versionB
                };
                return Task.FromResult(Json(book, HttpStatusCode.OK));
            }
            lastPut = request;

            if (getCount == 1)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict));
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
        });

        RegisterClient(handler);

        var cut = Render<EditBook>(parameters => parameters.Add(p => p.id, 1));

        await cut.InvokeAsync(async () =>
        {
            await cut.Find("form").SubmitAsync();
        });
        await cut.InvokeAsync(async () =>
        {
            await cut.Find("button.btn-primary").ClickAsync();
        });
        await cut.InvokeAsync(async () =>
        {
            await cut.Find("form").SubmitAsync();
        });

        var body = await lastPut!.Content!.ReadAsStringAsync();
        Assert.Contains(versionB.ToString(), body);
    }









    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _send;
        public FakeHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> send)
        {
            _send = send;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        => _send(request);
    }
}