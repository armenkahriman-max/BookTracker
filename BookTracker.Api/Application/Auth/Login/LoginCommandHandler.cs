using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Auth.Login;

public class LoginCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<Member> passwordHasher,
    JwtTokenGenerator tokenGenerator) : IHandler
{
    public async Task<LoginResponse?> Execute(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var member =
            await dbContext.Members
                .AsNoTracking()
                .SingleOrDefaultAsync(member =>
                    (string)member.Email == email);

        if (member is null)
        {
            return null;
        }

        var verification =
            passwordHasher.VerifyHashedPassword(
                member,
                member.PasswordHash,
                request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return tokenGenerator.Generate(member);
    }
}