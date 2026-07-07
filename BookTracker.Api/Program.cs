
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Endpoints;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Seeding;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<CreateBookCommandHandler>();
builder.Services.AddScoped<UpdateBookCommandHandler>();
builder.Services.AddScoped<DeleteBookCommandHandler>();
builder.Services.AddScoped<GetBookListQuery>();
builder.Services.AddScoped<GetBookByIdQuery>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker"));
});

builder.Services.AddScoped<IBookRepository, EfBookRepository>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureCreated();
        if (builder.Configuration.GetValue<bool>("SeedDatabase"))
            DatabaseSeeder.SeedBooks(dbContext, 500);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapBookEndpoints();

app.Run();

public partial class Program;