using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.GetBookDetails;

public class GetBookDetailsQueryHandler(AppDbContext dbContext) : IHandler
{
    public async Task<GetBookDetailsResponse?> Execute(int id)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
            .Select(book =>
                new GetBookDetailsResponse
                {
                    Id = book.Id,
                    Title = book.Title.Value,
                    Author = book.Author.Value,
                    Year = book.Year
                })
            .FirstOrDefaultAsync();
    }
}