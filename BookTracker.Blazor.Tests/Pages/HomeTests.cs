using Bunit;
using BookTracker.Blazor.Pages;
using Xunit;

namespace BookTracker.Blazor.Tests.Pages;

public class HomeTests : BunitContext
{
    [Fact]
    public void HidesAuthorsWhenToggleIsClicked()
    {
        var cut = Render<Home>();

        cut.Find("button").Click();

        Assert.DoesNotContain("Frank Herbert", cut.Markup);
        Assert.DoesNotContain("Raymond Chandler", cut.Markup);
    }
}