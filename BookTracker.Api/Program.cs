using BookTracker.Api.Wiring;

var builder = WebApplication.CreateBuilder(args);

builder.AddBookTracker();

// CORS - Only for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseBookTracker();
app.UseCors();           // Must be before endpoints

app.Run();

public partial class Program;