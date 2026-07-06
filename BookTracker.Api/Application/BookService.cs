using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Storage;
using BookTracker.Api.Domain;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Application.GetBookById;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{

    public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
    {
        var book = new Book
        {
            Title = new BookTitle(request.Title),
            Author = new AuthorName(request.Author),
            Year = request.Year
        };
        var savedBook = await bookRepository.AddAsync(book);

        return new CreateBookResponse
        {
            Id = savedBook.Id,
            Title = savedBook.Title.Value,
            Author = savedBook.Author.Value,
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
                Title = new BookTitle(request.Title),
                Author = new AuthorName(request.Author),
                Year = request.Year
            };

        return await bookRepository.UpdateAsync(book);
    }

}