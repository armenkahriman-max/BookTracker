namespace BookTracker.Api.Application.UpdateBook;

public class UpdateBookRequest
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public int Year { get; set; }
}