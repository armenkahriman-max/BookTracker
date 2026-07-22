using System.Security.Claims;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Endpoints;

public static class ClaimsPrincipalExtensions
{
    public static Actor ToActor(
        this ClaimsPrincipal principal)
    {
        var memberIdValue =
            principal.FindFirstValue(
                ClaimTypes.NameIdentifier);

        var roleValue =
            principal.FindFirstValue(
                ClaimTypes.Role);

        if (!int.TryParse(
            memberIdValue,
            out var memberId))
        {
            throw new InvalidOperationException(
                "Authenticated user has no valid member id.");
        }

        if (!Enum.TryParse<MemberRole>(
            roleValue,
            out var role))
        {
            throw new InvalidOperationException(
                "Authenticated user has no valid member role.");
        }

        return new Actor(
            memberId,
            role);
    }
}