using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace BookTracker.Api.Tests.IntegrationTests;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container =
        new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("booktracker_tests")
            .WithUsername("booktracker")
            .WithPassword("booktracker-tests")
            .Build();

    public string ConnectionString => container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.MigrateAsync();
    }

    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            TRUNCATE TABLE "Books", "Members"
            RESTART IDENTITY CASCADE;
            """;

        await command.ExecuteNonQueryAsync();
    }

    public Task DisposeAsync()
    {
        return container.DisposeAsync().AsTask();
    }
}