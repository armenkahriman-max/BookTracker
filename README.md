# BookTracker

## Running the tests

### Fast tests (no Docker needed)
Use these during normal daily development:

dotnet test BookTracker.Api.Tests/BookTracker.Api.Tests.csproj

### Integration tests (Docker required)
Use these when working on endpoints, database or migrations:

dotnet test BookTracker.Api.IntegrationTests/BookTracker.Api.IntegrationTests.csproj

### Run everything

dotnet test BookTracker.sln

Notes:
- Fast tests need no Docker.
- Integration tests start a temporary PostgreSQL container and require Docker to be running.