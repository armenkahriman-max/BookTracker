using BookTracker.Api.Domain.Members;
using BookTracker.Api.Endpoints;
using BookTracker.Api.Endpoints.Auth;
using BookTracker.Api.Seeding;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Wiring;

public static class WebApplicationExtensions // program.cs povezano da bude kracje
{
    public static WebApplication UseBookTracker(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var passwordHasher = scope.ServiceProvider
                .GetRequiredService<IPasswordHasher<Member>>();

            dbContext.Database.EnsureCreated();

            if (app.Configuration.GetValue<bool>("SeedDatabase"))
            {
                DatabaseSeeder.SeedBooks(dbContext, 500);

                DatabaseSeeder.SeedAdministrator(
                    dbContext,
                    app.Configuration,
                    passwordHasher);
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