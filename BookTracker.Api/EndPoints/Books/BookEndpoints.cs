using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Application.GetBookSummaries;
using BookTracker.Api.Application.GetBookDetails;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Security;
using System.Security.Claims;
using BookTracker.Api.Storage.Books;
using BookTracker.Api.Middleware;



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

    private static async Task<IResult> CreateBook(
     CreateBookRequest request,
     ClaimsPrincipal principal,
     CreateBookCommandHandler handler)
    {
        var response = await handler.Execute(principal.ToActor(), request);
        return Results.Created($"/books/{response.Id}", response);
    }


    private static async Task<IResult> UpdateBook(
     int id,
     UpdateBookRequest request,
     ClaimsPrincipal principal,
     UpdateBookCommandHandler handler)
    {
        var result = await handler.Execute(principal.ToActor(), id, request);


        return result switch
        {
            UpdateBookResult.Updated => Results.NoContent(),
            UpdateBookResult.NotFound => Results.NotFound(),
            UpdateBookResult.Conflict => Results.Conflict(
                new ErrorResponse("The book was changed by another user.")),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    
    private static async Task<IResult> DeleteBook(
               int id,
               ClaimsPrincipal principal,
               DeleteBookCommandHandler handler)
    {
       var deleted = await handler.Execute(principal.ToActor(), id);
       return deleted ? Results.NoContent() : Results.NotFound();
    }
}
