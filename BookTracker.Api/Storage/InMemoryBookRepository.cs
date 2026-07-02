using BookTracker.Api.Domain;

namespace BookTracker.Api.Storage;

public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> books = [];
    private int nextId = 1;

    public Task<IReadOnlyList<Book>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Book>>(books);
    }
    public Task<Book?> GetByIdAsync(int id)
    {
        var book = books.FirstOrDefault(book => book.Id == id);
        return Task.FromResult(book);
    }
    public Task<Book> AddAsync(Book book)
    {
        book.Id = nextId;
        nextId++;
        books.Add(book);
        return Task.FromResult(book);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var book = books.FirstOrDefault(book => book.Id == id);
        if (book is null)
        {
            return Task.FromResult(false);
        }

        books.Remove(book);
        return Task.FromResult(true);

    }


}