using BookTracker.Api.Application.GetMemberSummaries;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Application.GetMemberSummariesQueryHandler;


public class GetMemberSummariesQueryHandler(AppDbContext dbContext) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<PagedResult<MemberSummary>> Execute(
       Actor actor,
       GetMemberSummariesRequest request)
    {
        MemberPermissions.EnsureCanViewDirectory(actor);
        
        var page = Math.Max(1, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPage, MaxPageSize);

        var query = dbContext.Members.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search.Trim()}%";

            query = query.Where(member =>
                        EF.Functions.ILike((string)member.Name, search) ||
                        EF.Functions.ILike((string)member.Email, search));
        }

        var totalItems = await query.CountAsync();

        var members = await query
            .OrderBy(member => member.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(member =>
                new MemberSummary
                {
                    Id = member.Id,
                    Name = member.Name.Value,
                    Email = member.Email.Value
                })
            .ToListAsync();

        return
            new PagedResult<MemberSummary>
            {
                Items = members,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
    }
}