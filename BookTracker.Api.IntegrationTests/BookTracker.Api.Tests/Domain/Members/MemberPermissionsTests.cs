using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.Domain.Members;

public class MemberPermissionsTests
{
    [Fact]
    public void AdministratorCanViewDirectory()
    {
        var actor = new Actor(1, MemberRole.Administrator);
        MemberPermissions.EnsureCanViewDirectory(actor);
    }

    [Fact]
    public void MemberCannotViewDirectory()
    {
        var actor = new Actor(42, MemberRole.Member);

        Assert.Throws<ForbiddenOperationException>(() =>
            MemberPermissions.EnsureCanViewDirectory(actor));
    }

    [Fact]
    public void MemberCanManageOwnAccount()
    {
        var actor = new Actor(42, MemberRole.Member);
        MemberPermissions.EnsureCanManage(actor, 42);
    }

    [Fact]
    public void MemberCannotManageAnotherAccount()
    {
        var actor = new Actor(42, MemberRole.Member);

        Assert.Throws<ForbiddenOperationException>(() =>
            MemberPermissions.EnsureCanManage(actor, 99));
    }

    [Fact]
    public void AdministratorCanManageAnyAccount()
    {
        var actor = new Actor(1, MemberRole.Administrator);


        MemberPermissions.EnsureCanManage(actor, 1);


        MemberPermissions.EnsureCanManage(actor, 99);
    }
}
