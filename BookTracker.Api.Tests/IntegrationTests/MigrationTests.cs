using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Tests.IntegrationTests.Migrations;

public class MigrationTests
{
    [Fact]
    public async Task MigrationsCreateUsableDatabase()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new AppDbContext(options);

        await dbContext.Database.MigrateAsync();

        var appliedMigrations = await dbContext.Database
            .GetAppliedMigrationsAsync();

        Assert.Contains(
            appliedMigrations,
            migration => migration.EndsWith("_InitialCreate"));

        dbContext.Books.Add(new Book
        {
            Title = new BookTitle("Dune"),
            Author = new AuthorName("Frank Herbert"),
            Year = 1965
        });

        await dbContext.SaveChangesAsync();

        Assert.Equal(1, await dbContext.Books.CountAsync());
    }
}