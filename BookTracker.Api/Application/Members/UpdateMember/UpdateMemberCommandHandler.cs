using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;
using BookTracker.Api.Application.Members;
namespace BookTracker.Api.Application.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<bool> Execute(int id, UpdateMemberRequest request)
    {
        var name = new MemberName(request.Name);
        var email = new MemberEmail(request.Email);

        var EmailExists =
        await memberRepository.EmailExistsAsync(email, id);

        if (EmailExists)
        {
            throw new MemberEmailAlreadyExistsException();
        }

        var existingMember = await memberRepository.GetByIdAsync(id);
        if (existingMember is null)
        {
            return false;
        }

        
        existingMember.Name = name;
        existingMember.Email = email;

        return await memberRepository.UpdateAsync(existingMember);
    }


}
