namespace BookTracker.Blazor.Models.Books;

public sealed class BookDetailsResponse
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Author { get; set; }

    public int Year { get; set; }

    public Guid Version { get; set; }
}