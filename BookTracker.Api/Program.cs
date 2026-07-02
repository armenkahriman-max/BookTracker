using BookTracker.Api.Application;
using BookTracker.Api.Storage;
using BookTracker.Api.Application.CreateBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

builder.Services.AddScoped<BookService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/books", async (BookService service) => Results.Ok(await service.GetAllBooks()));

app.MapPost("/books", async (CreateBookRequest request, BookService service) =>
{
    var response = await service.CreateBook(request);
    return Results.Created($"/books/{response.Id}", response);
});


app.Run();

public partial class Program;