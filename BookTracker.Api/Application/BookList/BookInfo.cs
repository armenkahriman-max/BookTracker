using BookTracker.Api.Domain;

namespace BookTracker.Api.Application.BookList;

public class BookInfo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public int Year { get; set; }
}