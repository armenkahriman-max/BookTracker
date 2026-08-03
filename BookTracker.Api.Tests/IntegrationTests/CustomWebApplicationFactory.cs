using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace BookTracker.Api.Tests.IntegrationTests;

public class CustomWebApplicationFactory(PostgreSqlFixture database)
    : WebApplicationFactory<Program>
{
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
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(database.ConnectionString));
        });
    }

    public EfReader GetReader() => new(Services);
    public EfWriter GetWriter() => new(Services);
}