using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Seeding;

public static class DatabaseSeeder
{
    public static void SeedBooks(AppDbContext dbContext, int count = 50)
    {
        if (dbContext.Books.Any())
        {
            return;
        }

        var books = BookFuzzr.Many(count);

        dbContext.Books.AddRange(books);

        dbContext.SaveChanges();
    }

    public static void SeedAdministrator(
    AppDbContext dbContext,
    IConfiguration configuration,
    IPasswordHasher<Member> passwordHasher)
    {
        var settings =
            configuration
                .GetSection(
                    DevelopmentAdminSettings.SectionName)
                .Get<DevelopmentAdminSettings>();

        if (settings is null ||
            string.IsNullOrWhiteSpace(settings.Password))
        {
            return;
        }

        var email =
            new MemberEmail(settings.Email);

        var exists =
            dbContext.Members.Any(member =>
                (string)member.Email == email.Value);

        if (exists)
        {
            return;
        }

        var administrator =
            new Member
            {
                Name =
                    new MemberName(settings.Name),
                Email = email,
                PasswordHash = string.Empty,
                Role = MemberRole.Administrator
            };

        administrator.PasswordHash =
            passwordHasher.HashPassword(
                administrator,
                settings.Password);

        dbContext.Members.Add(administrator);
        dbContext.SaveChanges();
    }
}