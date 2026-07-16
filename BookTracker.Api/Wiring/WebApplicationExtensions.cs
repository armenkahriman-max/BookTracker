using BookTracker.Api.Endpoints;
using BookTracker.Api.Endpoints.Auth;
using BookTracker.Api.Seeding;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Wiring;

public static class WebApplicationExtensions
{
    public static WebApplication UseBookTracker(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.EnsureCreated();

            if (app.Configuration.GetValue<bool>("SeedDatabase"))
            {
                DatabaseSeeder.SeedBooks(dbContext, 500);
            }
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAuthEndpoints();
        app.MapBookEndpoints();
        app.MapMemberEndpoints();

        return app;
    }
}