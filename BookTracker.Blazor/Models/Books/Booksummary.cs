namespace BookTracker.Blazor.Models.Books;

public sealed class BookSummary
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
}