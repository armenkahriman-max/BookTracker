using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.IntegrationTests.IntegrationTests.Migrations;

[Collection(PostgreSqlCollection.Name)]
public class MigrationTests(PostgreSqlFixture database)
    : IntegrationTest(database)
{
    [Fact]
    public void MigrationsCreateUsablePostgreSqlDatabase()
    {
        var appliedMigrations = Reader.Query(db =>
            db.Database.GetAppliedMigrations().ToList());

        Assert.Contains(
            appliedMigrations,
            migration => migration.EndsWith("_InitialCreate"));

        Writer.Seed(db => db.Books.Add(new Book
        {
            Title = new BookTitle("Dune"),
            Author = new AuthorName("Frank Herbert"),
            Year = 1965
        }));

        Assert.Equal(1, Reader.Query(db => db.Books.Count()));
    }
}