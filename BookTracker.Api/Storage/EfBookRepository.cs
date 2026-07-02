using BookTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class EfBookRepository(AppDbContext dbContext) : IBookRepository
{
    public async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        return await dbContext.Books
        .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await dbContext.Books
        .FirstOrDefaultAsync(b => b.Id == id);
    }

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
}