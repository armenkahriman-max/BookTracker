namespace BookTracker.Api.Application.GetBookSummaries;

public class BookSummary
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public int Year { get; set; }
}