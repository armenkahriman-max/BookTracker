using BookTracker.Api.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Api.IntegrationTests.IntegrationTests;

public class EfWriter(IServiceProvider services)
{
    public void Seed(Action<AppDbContext> seed)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        seed(db);
        db.SaveChanges();
    }
}