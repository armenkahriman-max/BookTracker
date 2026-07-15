using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using Microsoft.EntityFrameworkCore;
namespace BookTracker.Api.Storage;

public class EfMemberRepository(AppDbContext dbContext) : IMemberRepository
{
    public async Task<Member> AddAsync(Member member)
    {
        dbContext.Members.Add(member);
        await dbContext.SaveChangesAsync();
        return member;
    }
    public async Task<bool> EmailExistsAsync(
    MemberEmail email,
    int? memberIdToIgnore = null)
    {
        return await dbContext.Members.AnyAsync(current =>
            current.Email == email &&
            (!memberIdToIgnore.HasValue || current.Id != memberIdToIgnore.Value));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var member = await dbContext.Members.FindAsync(id);
        if (member is null)
        {
            return false;
        }
        dbContext.Members.Remove(member);
        await dbContext.SaveChangesAsync();

        return true;

    }
    public async Task<bool> UpdateAsync(Member member)
    {
        var existingMember = await dbContext.Members.FindAsync(member.Id);
        if (existingMember is null)
        {
            return false;
        }
        existingMember.Name = member.Name;
        existingMember.Email = member.Email;


        await dbContext.SaveChangesAsync();

        return true;

    }
}
