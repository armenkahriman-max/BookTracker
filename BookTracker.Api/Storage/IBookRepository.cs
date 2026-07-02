using BookTracker.Api.Domain;

namespace BookTracker.Api.Storage;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> AddAsync(Book book);
    Task<bool> DeleteAsync(int id);
}