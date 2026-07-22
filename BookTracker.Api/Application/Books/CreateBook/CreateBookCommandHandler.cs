using BookTracker.Api.Domain;
using BookTracker.Api.Storage;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Application.CreateBook;

public class CreateBookCommandHandler(IBookRepository bookRepository) : IHandler
{
    public async Task<CreateBookResponse> Execute(
     Actor actor,
     CreateBookRequest request)
    {
        BookPermissions.EnsureCanManage(actor);

        var book =
            new Book
            {
                Title =
                    new BookTitle(request.Title),
                Author =
                    new AuthorName(request.Author),
                Year =
                    request.Year
            };

        var savedBook =
            await bookRepository.AddAsync(book);

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