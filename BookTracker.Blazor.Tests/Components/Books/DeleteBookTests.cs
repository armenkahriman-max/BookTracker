using System.Data.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AngleSharp.Io;
using AngleSharp.Svg.Dom;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Pages.Books;
using BookTracker.Blazor.Pages.Books.Edit;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Blazor.Tests.Components.Books;

public class DeleteBookTests : BunitContext
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
    public void Members_CannotSeeTheDeleteBtn()
    {
        var getBook = new BookDetailsResponse()
        {
            Id = 21,
            Title = "Test Title",
            Author = "Test Author",
            Year = 2024
        };


        var handler = new FakeHandler(_ =>
        Task.FromResult(Json(getBook)));

        RegisterClient(handler);
        var auth = AddAuthorization();
        auth.SetAuthorized("Bob");
        auth.SetRoles("Member");

        var cut = Render<BookDetails>(parameters => parameters.Add(p => p.Id, 21));
        Assert.DoesNotContain("Delete", cut.Markup);
    }
    [Fact]
    public void Administrator_SeesDeleteBtn()
    {
        var getBook = new BookDetailsResponse()
        {
            Id = 21,
            Title = "Test Title",
            Author = "Test Author",
            Year = 2024
        };

        var handler = new FakeHandler(_ =>
        Task.FromResult(Json(getBook)));

        RegisterClient(handler);
        var auth = AddAuthorization();
        auth.SetAuthorized("Frank");
        auth.SetRoles("Administrator");

        var cut = Render<BookDetails>(parameters => parameters.Add(p => p.Id, 21));
        Assert.Contains("Delete", cut.Markup);
    }

    [Fact]
    public void ConfirmationIncludesBookTitle()
    {
        var getBook = new BookDetailsResponse()
        {
            Id = 21,
            Title = "Test Title",
            Author = "Test Author",
            Year = 2024
        };

        var handler = new FakeHandler(_ => Task.FromResult(Json(getBook)));

        RegisterClient(handler);

        var auth = AddAuthorization();
        auth.SetAuthorized("Frank");
        auth.SetRoles("Administrator");

        var cut = Render<BookDetails>(parameters => parameters.Add(p => p.Id, 21));

        cut.Find("button.btn-danger").Click();

        Assert.Contains("Test Title", cut.Markup);
        Assert.Contains("Confirm", cut.Markup);
    }

    [Fact]
    public void StatusCode204NavigatesToTheList()
    {
        var getBook = new BookDetailsResponse()
        {
            Id = 21,
            Title = "Test Title",
            Author = "Test Author",
            Year = 2024
        };
        var handler = new FakeHandler(request =>
            {
                if (request.Method.Method == "DELETE")
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
                }

                return Task.FromResult(Json(getBook));
            });

        RegisterClient(handler);

        var auth = AddAuthorization();
        auth.SetAuthorized("Frank");
        auth.SetRoles("Administrator");

        var cut = Render<BookDetails>(parameters => parameters.Add(p => p.Id, 21));

        cut.Find("button.btn-danger").Click();

        cut.FindAll("button")
        .First(b => b.TextContent.Trim() == "Confirm")
        .Click();

        var navMan = Services.GetRequiredService<NavigationManager>();
        Assert.Equal("http://localhost/", navMan.Uri);
    }

    [Fact]
    public void StatusCode404IsHandledCorrectly()
    {
        var getBook = new BookDetailsResponse()
        {
            Id = 21,
            Title = "Test Title",
            Author = "Test Author",
            Year = 2024
        };

        var handler = new FakeHandler(request =>
        {
            if (request.Method.Method == "DELETE")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return Task.FromResult(Json(getBook));
        });

        RegisterClient(handler);

        var auth = AddAuthorization();
        auth.SetAuthorized("Frank");
        auth.SetRoles("Administrator");

        var cut = Render<BookDetails>(parameters => parameters.Add(p => p.Id, 21));

        cut.Find("button.btn-danger").Click();

        cut.FindAll("button")
        .First(b => b.TextContent.Trim() == "Confirm")
        .Click();

        Assert.Contains("This book may already have been deleted", cut.Markup);
        Assert.DoesNotContain("<h1>Test Title</h1>", cut.Markup);

    }

    [Fact]
    public void DoubleClick_DoesNotSendTwoDeletes()
    {
        var getBook = new BookDetailsResponse
        {
            Id = 21,
            Title = "Test Title",
            Author = "Test Author",
            Year = 2024
        };
        var tcs = new TaskCompletionSource<HttpResponseMessage>();
        var deleteCalls = 0;

        var handler = new FakeHandler(request =>
        {
            if (request.Method.Method == "DELETE")
            {
                deleteCalls++;
                return tcs.Task;
            }
            return Task.FromResult(Json(getBook));
        });

        RegisterClient(handler);

        var auth = AddAuthorization();
        auth.SetAuthorized("Frank");
        auth.SetRoles("Administrator");

        var cut = Render<BookDetails>(parameters => parameters.Add(p => p.Id, 21));

        cut.Find("button.btn-danger").Click();

        cut.FindAll("button")
        .First(b => b.TextContent.Trim() == "Confirm")
        .Click();

        var confirm = cut.FindAll("button")
        .First(b => b.TextContent.Trim() == "Confirm");
        var cancel = cut.FindAll("button")
        .First(b => b.TextContent.Trim() == "Cancel");

        Assert.True(confirm.HasAttribute("disabled"));
        Assert.True(cancel.HasAttribute("disabled"));

        confirm.Click();

        Assert.Equal(1, deleteCalls);

        tcs.SetResult(new HttpResponseMessage(HttpStatusCode.NoContent));


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