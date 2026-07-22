using BookTracker.Api.Application.Members.GetMemberDetailsResponse;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.GetMemberDetails;

public class GetMemberDetailsQueryHandler(AppDbContext dbContext) : IHandler
{
    public async Task<GetMemberDetailsResponse?> Execute(
    Actor actor,
    int id)
    {
        MemberPermissions.EnsureCanViewDirectory(
            actor);
        return await dbContext.Members
        .AsNoTracking()
        .Where(member => member.Id == id)
        .Select(member =>
        new GetMemberDetailsResponse
        {
            Id = member.Id,
            Name = member.Name.Value,
            Email = member.Email.Value
        })
        .FirstOrDefaultAsync();
    }
}