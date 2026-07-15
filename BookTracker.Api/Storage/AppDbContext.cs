using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Book>(book =>
        {
            book.Property(b => b.Title)
                .HasConversion(
                    title => title.Value,
                    value => new BookTitle(value))
                .HasMaxLength(BookTitle.MaxLength);

            book.Property(b => b.Author)
                .HasConversion(
                    author => author.Value,
                    value => new AuthorName(value))
                .HasMaxLength(AuthorName.MaxLength);
        });


        modelBuilder.Entity<Member>(member =>
        {
            member.HasIndex(current => current.Email)
      .IsUnique();

            member.Property(m => m.Email)
                .HasConversion(
                    email => email.Value,
                    value => new MemberEmail(value))
                .HasMaxLength(MemberEmail.MaxLength);

            member.Property(m => m.Name)
                .HasConversion(
                    name => name.Value,
                    value => new MemberName(value))
                .HasMaxLength(MemberName.MaxLength);
        });
    }
}