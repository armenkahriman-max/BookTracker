using System.Text.Json;
using BookTracker.Api.Application.Members;
using BookTracker.Api.Domain;
using BookTracker.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace BookTracker.Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task DomainExceptionReturnsBadRequest()
    {
        var exception = new DomainException("Invalid book data.");
        var context = await Execute(exception);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var response = await JsonSerializer.DeserializeAsync<ErrorResponse>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Equal("Invalid book data.", response!.Error);
    }

    private static async Task<DefaultHttpContext> Execute(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);
        return context;
    }

    [Fact]
    public async Task UnexpectedExceptionReturnsInternalServerError()
    {
        var exception = new InvalidOperationException("Secret internal details");
        var context = await Execute(exception);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Position = 0;

        var response = await JsonSerializer.DeserializeAsync<ErrorResponse>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Equal("An unexpected error occurred.", response!.Error);
        Assert.DoesNotContain("Secret internal details", response.Error);
    }
    [Fact]
    public static async Task MemberEmailAlreadyExistsReturnsConflict()
    {
        var exception = new MemberEmailAlreadyExistsException();
        var context = await Execute(exception);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);

        context.Response.Body.Position = 0;

        var response = await JsonSerializer.DeserializeAsync<ErrorResponse>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Equal("A member with this email already exists.", response!.Error);


    }
}