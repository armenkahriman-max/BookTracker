using System.Net;
using System.Text.Json;
using Xunit.Sdk;

namespace BookTracker.Api.Tests.IntegrationTests;

public static class HttpResponseAssertions
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public static async Task<T> ReadJsonAs<T>(
        this HttpResponseMessage response,
        HttpStatusCode expectedStatusCode)
    {
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == expectedStatusCode,
            $"""
             Expected status code:
             {expectedStatusCode}

             Actual status code:
             {response.StatusCode}

             Response body:
             {body}
             """);

        try
        {
            var result = JsonSerializer.Deserialize<T>(body, JsonOptions);

            Assert.NotNull(result);

            return result;
        }
        catch (JsonException exception)
        {
            throw new XunitException(
                $"""
                 Response had the expected status code, but could not be parsed as JSON.

                 Expected JSON type:
                 {typeof(T).Name}

                 Response body:
                 {body}

                 JSON error:
                 {exception.Message}
                 """);
        }
    }

    public static async Task ShouldHaveStatusCode(
        this HttpResponseMessage response,
        HttpStatusCode expectedStatusCode)
    {
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == expectedStatusCode,
            $"""
             Expected status code:
             {expectedStatusCode}

             Actual status code:
             {response.StatusCode}

             Response body:
             {body}
             """);
    }
}