using BookTracker.Api.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Api.Tests.IntegrationTests;

public class EfReader(IServiceProvider services)
{
    public T Query<T>(Func<AppDbContext, T> query)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return query(db);
    }
}