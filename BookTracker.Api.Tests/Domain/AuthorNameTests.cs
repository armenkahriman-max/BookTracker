using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.Domain;

public class AuthorNameTests
{
    [Fact]
    public void AuthorNameAcceptsValidName()
    {
        var author = new AuthorName("F. Scott Fitzgerald");

        Assert.Equal("F. Scott Fitzgerald", author.Value);
    }

    [Fact]
    public void AuthorNameTrimsValue()
    {
        var author = new AuthorName("F. Scott Fitzgerald");

        Assert.Equal("F. Scott Fitzgerald", author.Value);
    }

    [Fact]
    public void AuthorNameRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new AuthorName("   "));
        Assert.Equal("Author is required.", exception.Message);
    }

    [Fact]
    public void AuthorNameRejectsNameLongerThan100Characters()
    {
        var tooLong = new string('x', 101);
        var exception = Assert.Throws<DomainException>(() => new AuthorName(tooLong));
        Assert.Equal("Author cannot be longer than 100 characters.", exception.Message);
    }


}