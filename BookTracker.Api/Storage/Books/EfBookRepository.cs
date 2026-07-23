using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Storage.Books;
using Microsoft.EntityFrameworkCore;


namespace BookTracker.Api.Storage; //contact with database

public class EfBookRepository(AppDbContext dbContext) : IBookRepository
{

    public async Task<Book> AddAsync(Book book)
    {
        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();
        return book;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await dbContext.Books.FindAsync(id);

        if (book is null)
        {
            return false;
        }

        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<UpdateBookResult> UpdateAsync(
      Book book,
      Guid expectedVersion)
    {
        var existingBook = await dbContext.Books.FindAsync(book.Id);

        if (existingBook is null)
        {
            return UpdateBookResult.NotFound;
        }


        dbContext.Entry(existingBook)
            .Property(current => current.Version)
            .OriginalValue = expectedVersion;


        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Year = book.Year;
        existingBook.Version = Guid.NewGuid();

        try
        {
            await dbContext.SaveChangesAsync();
            return UpdateBookResult.Updated;
        }
        catch (DbUpdateConcurrencyException)
        {
            return UpdateBookResult.Conflict;
        }
    }
}