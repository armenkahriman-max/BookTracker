using BookTracker.Api.Storage;

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
}