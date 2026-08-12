using System.Net;
using System.Text;
using System.Text.Json;
using Bunit;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Pages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BookTracker.Blazor.Tests.Pages;

public class HomeTests : BunitContext
{
    private static GetBookSummariesResponse CreateResponse(
        IReadOnlyList<BookSummary>? items = null,
        int page = 1,
        int pageSize = 10,
        int totalItems = 0,
        int totalPages = 0)
    {
        return new GetBookSummariesResponse
        {
            Items = items ?? Array.Empty<BookSummary>(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    private static HttpResponseMessage CreateJsonResponse(GetBookSummariesResponse body)
    {
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private void RegisterClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new BookTrackerClient(httpClient));
    }

    [Fact]
    public void ShowsLoadingState()
    {
        var tcs = new TaskCompletionSource<HttpResponseMessage>();
        var handler = new FakeHttpMessageHandler(_ => tcs.Task);

        RegisterClient(handler);

        var cut = Render<Home>();

        Assert.Contains("Boeken laden...", cut.Markup);

        tcs.SetResult(CreateJsonResponse(CreateResponse()));
    }

    [Fact]
    public void ShowsEmptyState()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            Task.FromResult(CreateJsonResponse(CreateResponse())));

        RegisterClient(handler);

        var cut = Render<Home>();

        Assert.Contains("Geen boeken gevonden.", cut.Markup);
    }

    [Fact]
    public void ShowsErrorState()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            throw new HttpRequestException("API down"));

        RegisterClient(handler);

        var cut = Render<Home>();

        Assert.Contains(
            "Er is een fout opgetreden bij het laden van de boeken.",
            cut.Markup);
    }

    [Fact]
    public void ShowsBooksAndPaging()
    {
        var items = new List<BookSummary>
        {
            new() { Id = 1, Title = "Dune", Author = "Frank Herbert" },
            new() { Id = 2, Title = "The Big Sleep", Author = "Raymond Chandler" }
        };

        var response = CreateResponse(
            items: items,
            page: 1,
            pageSize: 10,
            totalItems: 20,
            totalPages: 2);

        var handler = new FakeHttpMessageHandler(_ =>
            Task.FromResult(CreateJsonResponse(response)));

        RegisterClient(handler);

        var cut = Render<Home>();

        Assert.Contains("Dune", cut.Markup);
        Assert.Contains("Frank Herbert", cut.Markup);
        Assert.Contains("Pagina 1 van 2", cut.Markup);

        var vorige = cut.FindAll("button")
            .First(b => b.TextContent.Contains("Vorige"));
        Assert.True(vorige.HasAttribute("disabled"));
    }

    [Fact]
    public void HidesAuthorsWhenToggleIsClicked()
    {
        var items = new List<BookSummary>
        {
            new() { Id = 1, Title = "Dune", Author = "Frank Herbert" },
            new() { Id = 2, Title = "The Big Sleep", Author = "Raymond Chandler" }
        };

        var response = CreateResponse(
            items: items,
            totalItems: 2,
            totalPages: 1);

        var handler = new FakeHttpMessageHandler(_ =>
            Task.FromResult(CreateJsonResponse(response)));

        RegisterClient(handler);

        var cut = Render<Home>();

        cut.FindAll("button")
            .First(b =>
                b.TextContent.Contains("Hide author") ||
                b.TextContent.Contains("Show author"))
            .Click();

        Assert.DoesNotContain("Frank Herbert", cut.Markup);
        Assert.DoesNotContain("Raymond Chandler", cut.Markup);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _send;

        public FakeHttpMessageHandler(
            Func<HttpRequestMessage, Task<HttpResponseMessage>> send)
        {
            _send = send;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => _send(request);
    }
}