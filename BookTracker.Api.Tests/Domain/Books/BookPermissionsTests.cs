using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Books;

public class BookPermissionsTests
{
    [Fact]
    public void AdministratorCanManageBooks()
    {
        var actor =
            new Actor(
                1,
                MemberRole.Administrator);

        BookPermissions.EnsureCanManage(actor);
    }
    [Fact]
    public void MemberCannotManageBooks()
    {
        var actor =
            new Actor(
                1,
                MemberRole.Member);

        Assert.Throws<ForbiddenOperationException>(
            () =>
                BookPermissions.EnsureCanManage(
                    actor));
    }
}