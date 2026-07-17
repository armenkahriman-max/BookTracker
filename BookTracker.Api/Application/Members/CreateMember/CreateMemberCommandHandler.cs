using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;
using BookTracker.Api.Application.Members;
namespace BookTracker.Api.Application.CreateMember;

public class CreateMemberCommandHandler(
    IMemberRepository memberRepository,
    IPasswordHasher<Member> passwordHasher) : IHandler
{

    public async Task<CreateMemberResponse> Execute(CreateMemberRequest request)
    {
        var name = new MemberName(request.Name);
        var email = new MemberEmail(request.Email);

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new DomainException("Password is required.");
        }

        if (request.Password.Length < 8)
        {
            throw new DomainException("Password must contain at least 8 characters.");
        }

        if (await memberRepository.EmailExistsAsync(email))
        {
            throw new MemberEmailAlreadyExistsException();
        }

        var member =
     new Member
     {
         Name = name,
         Email = email,
         PasswordHash = string.Empty,
         Role = MemberRole.Member
     };

        member.PasswordHash = passwordHasher.HashPassword(member, request.Password);

        var savedMember = await memberRepository.AddAsync(member);

        return new CreateMemberResponse
        {
            Id = savedMember.Id,
            Name = savedMember.Name.Value,
            Email = savedMember.Email.Value,
        };
    }
}