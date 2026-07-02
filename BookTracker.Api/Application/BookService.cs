using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Storage;
using BookTracker.Api.Domain;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Application.GetBookById;

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
    public async Task<bool> DeleteBook(int id)
    {
        return await bookRepository.DeleteAsync(id);
    }

    public async Task<bool> UpdateBook(int id, UpdateBookRequest request)
    {
        var book =
            new Book
            {
                Id = id,
                Title = request.Title,
                Author = request.Author,
                Year = request.Year
            };

        return await bookRepository.UpdateAsync(book);
    }


    public async Task<BookDetails?> GetBookById(int id)
{
    var book = await bookRepository.GetByIdAsync(id);

    if (book is null)
    {
        return null;
    }

    return
        new BookDetails
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Year = book.Year
        };
}
}