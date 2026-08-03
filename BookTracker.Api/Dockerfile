# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BookTracker.Api/BookTracker.Api.csproj BookTracker.Api/
RUN dotnet restore BookTracker.Api/BookTracker.Api.csproj

COPY BookTracker.Api/ BookTracker.Api/
RUN dotnet publish BookTracker.Api/BookTracker.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN mkdir /data && chown app:app /data

COPY --from=build /app/publish .

USER app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BookTracker.Api.dll"]