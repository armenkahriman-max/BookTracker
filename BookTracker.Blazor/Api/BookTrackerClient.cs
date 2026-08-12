using System.Net.Http.Json;
using BookTracker.Blazor.Models.Books;
using System.Net;

namespace BookTracker.Blazor.Api;

public sealed class BookTrackerClient(HttpClient httpClient)
{
    public async Task<GetBookSummariesResponse> GetBooks(
        string? search,
        int page,
        int pageSize)
    {
        var url = $"/books?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }
        return await httpClient.GetFromJsonAsync<GetBookSummariesResponse>(url)
        ?? throw new InvalidOperationException("Book list response was empty.");
    }

    public async Task<BookDetailsResponse?> GetBookDetails(int id)
    {
        using var response = await httpClient.GetAsync($"/books/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BookDetailsResponse>()
        ?? throw new InvalidOperationException("Book details response was empty.");
    }
}