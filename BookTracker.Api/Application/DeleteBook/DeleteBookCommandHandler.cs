using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.DeleteBook;

public class DeleteBookCommandHandler(IBookRepository bookRepository)
{
    public async Task<bool> Execute(int id)
    {
        return await bookRepository.DeleteAsync(id);
    }
}