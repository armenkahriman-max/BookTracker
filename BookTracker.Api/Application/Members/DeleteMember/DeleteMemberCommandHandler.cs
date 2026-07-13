using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.DeleteMember;

public class DeleteMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<bool> Execute(int id)
    {
        return await memberRepository.DeleteAsync(id);
    }
}