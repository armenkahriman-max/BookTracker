using BookTracker.Api.Domain;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.CreateBook;

public class CreateBookCommandHandler(IBookRepository bookRepository)
{
    public async Task<CreateBookResponse> Execute(CreateBookRequest request)
    {
        var book =
            new Book
            {
                Title = new BookTitle(request.Title),
                Author = new AuthorName(request.Author),
                Year = request.Year
            };

        var savedBook = await bookRepository.AddAsync(book);

        return
            new CreateBookResponse
            {
                Id = savedBook.Id,
                Title = savedBook.Title.Value,
                Author = savedBook.Author.Value,
                Year = savedBook.Year
            };
    }
}