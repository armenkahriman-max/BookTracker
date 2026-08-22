using Bunit;
using BookTracker.Blazor.Layout;

namespace BookTracker.Blazor.Tests.Pages;

public class NavMenuTests : BunitContext
{
    [Fact]
    public void Anonymous_User_Sees_Login_And_Register()
    {
        var authContext = this.AddAuthorization();
        authContext.SetNotAuthorized();

        var cut = Render<NavMenu>();

        Assert.Contains("Login", cut.Markup);
        Assert.Contains("Register", cut.Markup);
        Assert.DoesNotContain("Logout", cut.Markup);
        Assert.DoesNotContain("Add Book", cut.Markup);

    }

    [Fact]
    public void LoggedIn_User_Sees_Logout_And_Account()
    {
        var authContext = this.AddAuthorization();
        authContext.SetAuthorized("Ada Reader");

        var cut = Render<NavMenu>();

        Assert.Contains("Logout", cut.Markup);
        Assert.Contains("My Account", cut.Markup);
        Assert.DoesNotContain("Login", cut.Markup);
        Assert.DoesNotContain("Add Book", cut.Markup);
    }

    [Fact]
    public void Administrator_Sees_Add_Book()
    {
        var authContext = this.AddAuthorization();
        authContext.SetAuthorized("Book Tracker Admin");
        authContext.SetRoles("Administrator");

        var cut = Render<NavMenu>();

        Assert.Contains("Add Book", cut.Markup);
        Assert.Contains("Logout", cut.Markup);
    }

    [Fact]
    public void Ordinary_Member_Does_Not_See_Add_Book()
    {
        var authContext = this.AddAuthorization();
        authContext.SetAuthorized("Ordinary Member");

        var cut = Render<NavMenu>();

        Assert.DoesNotContain("Add Book", cut.Markup);
        Assert.Contains("Logout", cut.Markup);
    }

    [Fact]
    private void Logout_Changes_Authentication_State()
    {
        var authContext = this.AddAuthorization();
        authContext.SetAuthorized("Ada Reader");

        var cut = Render<NavMenu>();

        Assert.Contains("Logout", cut.Markup);
        Assert.DoesNotContain("Login", cut.Markup);
        Assert.DoesNotContain("Register", cut.Markup);

        cut.FindAll("a, button")
        .First(e => e.TextContent.Contains("Logout"))
        .Click();

        authContext.SetNotAuthorized();
        cut.Render();

        Assert.Contains("Login", cut.Markup);
        Assert.Contains("Register", cut.Markup);
        Assert.DoesNotContain("Logout", cut.Markup);
        Assert.DoesNotContain("My Account", cut.Markup);
        Assert.DoesNotContain("Add Book", cut.Markup);

    }
}