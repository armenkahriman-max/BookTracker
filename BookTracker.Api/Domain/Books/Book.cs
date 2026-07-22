namespace BookTracker.Api.Domain.Books;

public class Book
{
    public int Id { get; set; }

    public required BookTitle Title { get; set; }

    public required AuthorName Author { get; set; }

    public int Year { get; set; }

    public Guid Version { get; set; } = Guid.NewGuid();
}