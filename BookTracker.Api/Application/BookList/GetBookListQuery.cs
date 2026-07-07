using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(AppDbContext dbContext)
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<PagedResult<BookInfo>> Execute(GetBookListRequest request)
    {
        var page = Math.Max(1, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPage, MaxPageSize);

        var totalItems = await dbContext.Books.CountAsync();

        var books = await dbContext.Books
            .AsNoTracking()
            .OrderBy(book => book.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(book =>
                new BookInfo
                {
                    Id = book.Id,
                    Title = book.Title.Value,
                    Author = book.Author.Value
                })
            .ToListAsync();

        return
            new PagedResult<BookInfo>
            {
                Items = books,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
    }
}