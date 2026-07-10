using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Storage;

public interface IMemberRepository
{
    Task<Member> AddAsync(Member member);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(Member member);
}