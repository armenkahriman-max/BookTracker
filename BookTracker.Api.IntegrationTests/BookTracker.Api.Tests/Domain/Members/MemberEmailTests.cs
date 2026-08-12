using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.Domain;

public class MemberEmailTests
{
    [Fact]
    public void MemberEmailAcceptsValidEmailAddresses()
    {
        var email = new MemberEmail("armen.vdab@gmail.com");
        Assert.Equal("armen.vdab@gmail.com", email.Value);
    }

    [Fact]
    public void MemberEmailTrimsValue()
    {
        var email = new MemberEmail("  armen.vdab@gmail.com  ");
        Assert.Equal("armen.vdab@gmail.com", email.Value);
    }

    [Fact]
    public void MemberEmailRejectsWhitespaces()
    {
        var exception = Assert.Throws<DomainException>(() =>
        new MemberEmail("   "));
        Assert.Equal("Email is required.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectsEmailsLongerThan200Characters()
    {
        var tooLong = new string('x', 201) + "@";

        var exception = Assert.Throws<DomainException>(() =>
        new MemberEmail(tooLong));
        Assert.Equal("Email cannot contain more than 200 characters.", exception.Message);
    }
    [Fact]
    public void MemberEmailRejectsTextWithoutAtSymbol()
    {
        var exception = Assert.Throws<DomainException>(() =>
        new MemberEmail("thisemailhasnoat"));

        Assert.Equal("Email must contain an @ symbol.", exception.Message);


    }
    [Fact]
    public void MemberEmailNormalizesValue()
    {
        var email = new MemberEmail("  Ada@Example.com  ");

        Assert.Equal("ada@example.com", email.Value);
    }
}