using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Domain.Books;

public static class BookPermissions
{
    public static void EnsureCanManage(
        Actor actor)
    {
        if (actor.Role == MemberRole.Administrator)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot manage books.");
    }
}