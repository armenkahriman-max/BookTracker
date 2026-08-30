namespace BookTracker.Api.IntegrationTests.IntegrationTests;

[CollectionDefinition(
    "PostgreSQL integration tests",
    DisableParallelization = true)]
public class PostgreSqlCollection
    : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "PostgreSQL integration tests";
}