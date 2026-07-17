using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Storage;

public interface IMemberRepository
{
    Task<Member> AddAsync(Member member);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(Member member);

    Task<bool> EmailExistsAsync(
           MemberEmail email,
           int? memberIdToIgnore = null);
    Task<Member> GetByIdAsync(int id);
}