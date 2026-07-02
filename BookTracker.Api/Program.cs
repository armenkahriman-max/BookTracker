using BookTracker.Api.Application;
using BookTracker.Api.Storage;
using BookTracker.Api.Application.CreateBook;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Application.UpdateBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker"));
});

builder.Services.AddScoped<IBookRepository, EfBookRepository>();

builder.Services.AddScoped<BookService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/books", async (BookService service) => Results.Ok(await service.GetAllBooks()));

app.MapGet("/books/{id:int}", async (int id, BookService service) =>
{
    var book = await service.GetBookById(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(book);
});

app.MapPost("/books", async (CreateBookRequest request, BookService service) =>
{
    var response = await service.CreateBook(request);
    return Results.Created($"/books/{response.Id}", response);
});

app.MapDelete("/books/{id:int}", async (int id, BookService service) =>
{
    var deleted = await service.DeleteBook(id);

    if (!deleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.MapPut("/books/{id:int}", async (int id, UpdateBookRequest request, BookService service) =>
{
    var updated = await service.UpdateBook(id, request);

    if (!updated)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});


app.Run();

public partial class Program;