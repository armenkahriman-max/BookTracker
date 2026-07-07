using BookTracker.Api.Domain;

namespace BookTracker.Api.Storage;

public interface IBookRepository
{
    Task<Book> AddAsync(Book book);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(Book book);
}