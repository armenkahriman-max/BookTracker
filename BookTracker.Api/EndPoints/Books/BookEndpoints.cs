using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Application.GetBookSummaries;
using BookTracker.Api.Application.GetBookDetails;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;



namespace BookTracker.Api.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {


        app.MapGet("/books", GetBookSummaries);
        app.MapGet("/books/{id:int}", GetBookDetails);

        app.MapPost("/books", CreateBook)
     .RequireAuthorization();

        app.MapPut("/books/{id:int}", UpdateBook)
            .RequireAuthorization();

        app.MapDelete("/books/{id:int}", DeleteBook)
            .RequireAuthorization();
        return app;
    }

    public static async Task<IResult> GetBookSummaries(
     [AsParameters] GetBookSummariesRequest request,
     GetBookSummariesQueryHandler query)
    {
        var books = await query.Execute(request);

        return Results.Ok(books);
    }

    public static async Task<IResult> GetBookDetails(int id, GetBookDetailsQueryHandler query)
    {
        var book = await query.Execute(id);
        if (book is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(book);
    }

    public static async Task<IResult> CreateBook(
        CreateBookRequest request,
        CreateBookCommandHandler handler)
    {
        try
        {
            var response = await handler.Execute(request);
            return Results.Created($"/books/{response.Id}", response);
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    public static async Task<IResult> UpdateBook(
    int id,
    UpdateBookRequest request,
    UpdateBookCommandHandler handler)
    {
        try
        {
            var updated = await handler.Execute(id, request);

            if (!updated)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    public static async Task<IResult> DeleteBook(
     int id,
     DeleteBookCommandHandler handler)
    {
        try
        {
            var deleted = await handler.Execute(id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }
}