using System.Net.Http.Json;
using BookTracker.Blazor.Models.Books;
using System.Net;
using BookTracker.Blazor.Models.Auth;
using BookTracker.Blazor.Models.Books.Create;
using BookTracker.Blazor.Models.Books.Update;

namespace BookTracker.Blazor.Api;

public sealed class BookTrackerClient(HttpClient httpClient)
{

    public async Task<UpdateBookResult> UpdateBookAsync(int id, UpdateBookRequest request)
    {
        using var response = await httpClient.PutAsJsonAsync($"/books/{id}", request);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new UpdateBookResult(UpdateBookStatus.Updated);
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return new UpdateBookResult(UpdateBookStatus.ValidationFailed,
            ErrorMessage: "Failed to update book.");
        }
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new UpdateBookResult(UpdateBookStatus.Unauthorized);
        }
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new UpdateBookResult(UpdateBookStatus.Forbidden);
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new UpdateBookResult(UpdateBookStatus.NotFound,
            ErrorMessage: "Book not found.");
        }
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return new UpdateBookResult(UpdateBookStatus.Conflict);
        }
        return new UpdateBookResult(UpdateBookStatus.ValidationFailed,
        ErrorMessage: "Unexpected error while creating the book.");
    }






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

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        using var response = await httpClient.PostAsJsonAsync("auth/login", request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<CreateBookResult> CreateBookAsync(CreateBookRequest request)
    {
        using var response = await httpClient.PostAsJsonAsync("/books", request);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var book = await response.Content.ReadFromJsonAsync<CreateBookResponse>();
            return new CreateBookResult(CreateBookStatus.Created, Book: book);
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return new CreateBookResult(CreateBookStatus.ValidationFailed,
            ErrorMessage: "Failed to create book.");
        }
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new CreateBookResult(CreateBookStatus.Unauthorized);
        }
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new CreateBookResult(CreateBookStatus.Forbidden);
        }
        return new CreateBookResult(CreateBookStatus.ValidationFailed,
        ErrorMessage: "Unexpected error while creating the book.");


    }

    public async Task<DeleteBookResult> DeleteBookAsync(int id)
    {
        using var response = await httpClient.DeleteAsync($"/books/{id}");

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new DeleteBookResult(DeleteBookStatus.NoContent);
        }
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new DeleteBookResult(DeleteBookStatus.Unauthorized);
        }
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new DeleteBookResult(DeleteBookStatus.Forbidden);
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new DeleteBookResult(DeleteBookStatus.NotFound,
            ErrorMessage: "Book not found.");
        }
        return new DeleteBookResult(DeleteBookStatus.Error,
        ErrorMessage: "An unexpected Error accured.");
    }
}
public enum CreateBookStatus
{
    Created,
    ValidationFailed,
    Unauthorized,
    Forbidden
}
public enum UpdateBookStatus
{
    Updated,
    ValidationFailed,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict
}

public enum DeleteBookStatus
{
    Unauthorized,
    NoContent,
    Forbidden,
    NotFound,
    Error

}
public sealed record CreateBookResult(
    CreateBookStatus Status,
    CreateBookResponse? Book = null,
    string? ErrorMessage = null);

public sealed record UpdateBookResult(
    UpdateBookStatus Status,
    UpdateBookResponse? Book = null,
    string? ErrorMessage = null);

public sealed record DeleteBookResult(
    DeleteBookStatus Status,
    string? ErrorMessage = null);




