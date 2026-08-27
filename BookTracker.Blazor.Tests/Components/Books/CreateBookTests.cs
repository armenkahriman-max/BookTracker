using Bunit;
using BookTracker.Blazor.Api;
using System.Net;
using BookTracker.Blazor.Models.Books.Create;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using BookTracker.Blazor.Pages.Books.Create;
using Microsoft.AspNetCore.Components;


namespace BookTracker.Blazor.Tests.Components.Books;

public class CreateBookTests : BunitContext
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
    public async Task SubmitWithValidInput_NavigatesToDetails()
    {
        var createdBook = new CreateBookResponse()
        {
            Id = 42,
            Title = "Test Title",
            Author = "Test Author",
            Year = 2024
        };
        var handler = new FakeHandler(_ =>
        Task.FromResult(Json(createdBook, HttpStatusCode.Created)));

        RegisterClient(handler);

        var cut = Render<AdminPage>();


        await cut.InvokeAsync(async () =>
        {
            await cut.FindAll("input")[0].ChangeAsync("Test Title");
            await cut.FindAll("input")[1].ChangeAsync("Test Author");
            await cut.FindAll("input")[2].ChangeAsync("2024");

            await cut.Find("form").SubmitAsync();
        });

        var navMan = Services.GetRequiredService<NavigationManager>();
        Assert.Equal("http://localhost/books/42", navMan.Uri);
    }

    [Fact]
    public async Task Error400IsShownOnThePage()
    {

        var handler = new FakeHandler(_ =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

        RegisterClient(handler);

        var cut = Render<AdminPage>();

        await cut.InvokeAsync(async () =>
       {
           await cut.FindAll("input")[0].ChangeAsync("Test Title");
           await cut.FindAll("input")[1].ChangeAsync("Test Author");
           await cut.FindAll("input")[2].ChangeAsync("2024");

           await cut.Find("form").SubmitAsync();
       });
        Assert.Contains("Failed to create book.", cut.Markup);

    }
    [Fact]
    public async Task SubmitButtonIsDisabledWhileSubmitting()
    {
        var tcs = new TaskCompletionSource<HttpResponseMessage>();

        var handler = new FakeHandler(_ => tcs.Task);
        RegisterClient(handler);

        var cut = Render<AdminPage>();

        await cut.InvokeAsync(async () =>
       {
           await cut.FindAll("input")[0].ChangeAsync("Test Title");
           await cut.FindAll("input")[1].ChangeAsync("Test Author");
           await cut.FindAll("input")[2].ChangeAsync("2024");
       });

        var submitTask = cut.Find("form").SubmitAsync();

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));

        tcs.SetResult(Json(new CreateBookResponse { Id = 1, Title = "T", Author = "A", Year = 2024 }, HttpStatusCode.Created));

        await submitTask;
    }
    [Fact]
    public void HasAdministratorAuthorizeAttribute()
    {
        var attribute = typeof(AdminPage)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(attribute);
        Assert.Equal("Administrator", attribute.Roles);
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
