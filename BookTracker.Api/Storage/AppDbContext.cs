using BookTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
}