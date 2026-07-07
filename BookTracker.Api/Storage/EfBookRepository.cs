using BookTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

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

    public async Task<bool> UpdateAsync(Book book)
    {
        var existingBook = await dbContext.Books.FindAsync(book.Id);

        if (existingBook is null)
        {
            return false;
        }

        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Year = book.Year;

        await dbContext.SaveChangesAsync();

        return true;
    }
}