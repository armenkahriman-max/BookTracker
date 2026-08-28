using System.Net;
using System.Text;
using System.Text.Json;
using Bunit;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Pages.Books;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace BookTracker.Blazor.Tests.Pages;

public class BookDetailsTests : BunitContext
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
    public void UsesRouteParameterAndShowsBook()
    {
        var details = new BookDetailsResponse
        {
            Id = 42,
            Title = "Dune",
            Author = "Frank Herbert",
            Year = 1965,
            Version = Guid.NewGuid()
        };

        var handler = new FakeHandler(_ => Task.FromResult(Json(details)));
        RegisterClient(handler);
        AddAuthorization();

        var cut = Render<BookDetails>(p => p.Add(c => c.Id, 42));

        Assert.Contains("Dune", cut.Markup);
        Assert.Contains("Frank Herbert", cut.Markup);
        Assert.Contains("1965", cut.Markup);
    }

    [Fact]
    public void ShowsNotFoundFor404()
    {
        var handler = new FakeHandler(_ =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

        RegisterClient(handler);
        AddAuthorization();

        var cut = Render<BookDetails>(p => p.Add(c => c.Id, 999));

        Assert.Contains("Book not found", cut.Markup);
    }

    [Fact]
    public void ShowsErrorOnFailure()
    {
        var handler = new FakeHandler(_ =>
        throw new HttpRequestException("down"));

        RegisterClient(handler);
        AddAuthorization();

        var cut = Render<BookDetails>(p => p.Add(c => c.Id, 1));

        Assert.Contains("There was an error while loading the book.", cut.Markup);
    }

    [Fact]
    public void ReloadsWhenIdChanges()
    {
        var callCount = 0;

        var handler = new FakeHandler(req =>
        {
            callCount++;
            var idSegment = req.RequestUri!.Segments.Last().TrimEnd('/');
            var id = int.Parse(idSegment);

            var details = new BookDetailsResponse
            {
                Id = id,
                Title = $"Book {id}",
                Author = "Author",
                Year = 2000,
                Version = Guid.NewGuid()
            };
            return Task.FromResult(Json(details));
        });

        RegisterClient(handler);
        AddAuthorization();

        var cut = Render<BookDetails>(p => p.Add(c => c.Id, 1));
        Assert.Contains("Book 1", cut.Markup);

        cut.Render(p => p.Add(c => c.Id, 2));
        Assert.Contains("Book 2", cut.Markup);
        Assert.True(callCount >= 2);
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
