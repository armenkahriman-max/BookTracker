
namespace BookTracker.Api.Domain;

public class MemberNameTest
{
    [Fact]
    public void MemberNameAcceptsValidName()
    {
        var member = new MemberName("Desingerica Ludak");
        Assert.Equal("Desingerica Ludak", member.Value);
    }

    [Fact]
    public void MemberNameTrimsValue()
    {
        var member = new MemberName("Desingerica Ludak");
        Assert.Equal("Desingerica Ludak", member.Value);
    }

    [Fact]
    public void MemberNameRejectsWhitespaces()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberName("   "));
        Assert.Equal("Member name is required.", exception.Message);
    }

    [Fact]
    public void MemberNameRejectsNameLongerThan100Characters()
    {
        var tooLong = new string('x', 101);
        var exception = Assert.Throws<DomainException>(() => new MemberName(tooLong));
        Assert.Equal("Member name cannot be longer then 100 characters.", exception.Message);
    }
}