namespace BookTracker.Blazor.Models.Books.Update;

public sealed class UpdateBookResponse
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int Year { get; set; }
}