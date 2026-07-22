using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Domain.Actors;

public record Actor(
    int MemberId,
    MemberRole Role)
{
    public bool IsAdministrator =>
        Role == MemberRole.Administrator;
}