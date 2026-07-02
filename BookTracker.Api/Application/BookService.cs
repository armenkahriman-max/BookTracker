using BookTracker.Api.Application.BookList;
using BookTracker.Api.Storage;
using Microsoft.VisualBasic;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> GetAllBooks()
    {
        var book = await bookRepository.GetAllAsync();
        var summary = book.Select(book => new BookInfo
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author

        }).ToList();

        return summary;
    }
}