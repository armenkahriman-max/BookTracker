using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Storage;
using BookTracker.Api.Domain;

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

    public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Year = request.Year
        };
        var savedBook = await bookRepository.AddAsync(book);

        return new CreateBookResponse
        {
            Id = savedBook.Id,
            Title = savedBook.Title,
            Author = savedBook.Author,
            Year = savedBook.Year
        };
    }
}