using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.GetBookById;

public class GetBookByIdQuery(AppDbContext dbContext)
{
    public async Task<BookDetails?> Execute(int id)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
            .Select(book =>
                new BookDetails
                {
                    Id = book.Id,
                    Title = book.Title.Value,
                    Author = book.Author.Value,
                    Year = book.Year
                })
            .FirstOrDefaultAsync();
    }
}