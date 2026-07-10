using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.CreateMember;

public class CreateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<CreateMemberResponse> Execute(CreateMemberRequest request)
    {
        var member =
            new Member
            {
                Name = new MemberName(request.Name),
                Email = new MemberEmail(request.Email),
            };

        var savedMember = await memberRepository.AddAsync(member);

        return
            new CreateMemberResponse
            {
                Id = savedMember.Id,
                Name = savedMember.Name.Value,
                Email = savedMember.Email.Value,
            };
    }
}