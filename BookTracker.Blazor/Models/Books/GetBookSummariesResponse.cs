namespace BookTracker.Blazor.Models.Books;

public sealed class GetBookSummariesResponse
{
    public required IReadOnlyList<BookSummary> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}