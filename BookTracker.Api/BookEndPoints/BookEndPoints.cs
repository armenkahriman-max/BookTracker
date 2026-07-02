using BookTracker.Api.Application;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.UpdateBook;

namespace BookTracker.Api.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/books", GetAllBooks);
        app.MapGet("/books/{id:int}", GetBookById);
        app.MapPost("/books", CreateBook);
        app.MapPut("/books/{id:int}", UpdateBook);
        app.MapDelete("/books/{id:int}", DeleteBook);
        return app;
    }

    public static async Task<IResult> GetAllBooks(BookService service)
    {
        var books = await service.GetAllBooks();
        return Results.Ok(books);
    }

    public static async Task<IResult> GetBookById(int id, BookService service)
    {
        var book = await service.GetBookById(id);

        if (book is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(book);
    }

    public static async Task<IResult> CreateBook(CreateBookRequest request, BookService service)
    {
        var response = await service.CreateBook(request);
        return Results.Created($"/books/{response.Id}", response);
    }

    public static async Task<IResult> UpdateBook(int id, UpdateBookRequest request, BookService service)
    {
        var updated = await service.UpdateBook(id, request);

        if (!updated)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }

    public static async Task<IResult> DeleteBook(int id, BookService service)
    {
        var deleted = await service.DeleteBook(id);

        if (!deleted)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}