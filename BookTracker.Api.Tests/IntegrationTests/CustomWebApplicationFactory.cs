using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookTracker.Api.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? connection;

    private static readonly KeyValuePair<string, string?>[] TestSettings =
    [
        new("SeedDatabase", "false"),
        new("Jwt:Issuer", "BookTracker.Tests"),
        new("Jwt:Audience", "BookTracker.Tests"),
        new("Jwt:SigningKey", "book-tracker-test-signing-key-with-32-characters"),
        new("Jwt:ExpirationMinutes", "10")
    ];

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configuration =>
            configuration.AddInMemoryCollection(TestSettings));

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["SeedDatabase"] = "false"
                });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(service =>
                service.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connection));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            connection?.Dispose();
        }

        base.Dispose(disposing);
    }

    public EfReader GetReader() => new(Services);
    public EfWriter GetWriter() => new(Services);
}