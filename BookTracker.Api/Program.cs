using BookTracker.Api.Wiring;


var builder = WebApplication.CreateBuilder(args);

builder.AddBookTracker();



// CORS - Only for development
var frontendOrigin = builder.Configuration["FrontendOrigin"]
?? "http://localhost:5242";
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.
        WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod();

    });
});

var app = builder.Build();

app.UseBookTracker();
app.UseCors();           // Must be before endpoints



app.Run();

public partial class Program;