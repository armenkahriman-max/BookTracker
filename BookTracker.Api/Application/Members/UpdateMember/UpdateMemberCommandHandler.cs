using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<bool> Execute(int id, UpdateMemberRequest request)
    {
        var member = new Member
        {
            Id = id,
            Name = new MemberName(request.Name),
            Email = new MemberEmail(request.Email),
        };

        return await memberRepository.UpdateAsync(member);
    }
}