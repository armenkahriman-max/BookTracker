namespace BookTracker.Blazor.Models.Books.Update;

public sealed class UpdateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public Guid Version { get; set; }
}