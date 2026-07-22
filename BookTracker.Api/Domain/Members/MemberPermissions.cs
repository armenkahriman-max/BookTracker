using BookTracker.Api.Domain.Actors;

namespace BookTracker.Api.Domain.Members;

public static class MemberPermissions
{
    public static void EnsureCanViewDirectory(Actor actor)
    {
        if (actor.IsAdministrator)
            return;

        throw new ForbiddenOperationException("This actor cannot view the member directory.");
    }

    public static void EnsureCanManage(Actor actor, int memberId)
    {
        if (actor.IsAdministrator)
            return;

        if (actor.MemberId == memberId)
            return;

        throw new ForbiddenOperationException("This actor cannot manage this member.");
    }
}