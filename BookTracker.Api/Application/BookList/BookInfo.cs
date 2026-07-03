using BookTracker.Api.Domain;

namespace BookTracker.Api.Application.BookList;

public class BookInfo
{
    public int Id {get; set; }
    public required BookTitle Title {get; set; }
    public required AuthorName Author {get; set; }
}