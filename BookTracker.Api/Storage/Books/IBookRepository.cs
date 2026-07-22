using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Storage.Books;

namespace BookTracker.Api.Storage;

public interface IBookRepository
{
    Task<Book> AddAsync(Book book);
    Task<bool> DeleteAsync(int id);
    Task<UpdateBookResult> UpdateAsync(Book book, Guid expectedVersion);
}