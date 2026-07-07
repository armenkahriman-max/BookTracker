using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<BookInfo>> Execute()
    {
        return await dbContext.Books
            .AsNoTracking()
            .Select(book =>
                new BookInfo
                {
                    Id = book.Id,
                    Title = book.Title.Value,
                    Author = book.Author.Value
                })
            .ToListAsync();
    }
}