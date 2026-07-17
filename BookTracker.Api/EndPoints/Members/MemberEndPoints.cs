using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Application.DeleteMember;
using BookTracker.Api.Application.GetMemberDetails;
using BookTracker.Api.Application.GetMemberSummaries;
using BookTracker.Api.Application.GetMemberSummariesQueryHandler;
using BookTracker.Api.Application.Members;
using BookTracker.Api.Application.UpdateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using System.Security.Claims;

namespace BookTracker.Api.Endpoints;

public static class MemberEndPoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/members", GetMemberSummaries)
        .RequireAuthorization(
         AuthorizationPolicies.ManageMembers);

        app.MapGet("/members/{id:int}", GetMemberDetails)
        .RequireAuthorization(
        AuthorizationPolicies.ManageMembers);

        app.MapPost("/members", CreateMember);

        app.MapPut("/members/{id:int}", UpdateMember)
            .RequireAuthorization();

        app.MapDelete("/members/{id:int}", DeleteMember)
            .RequireAuthorization();

        return app;
    }


    public static async Task<IResult> GetMemberSummaries(
        [AsParameters] GetMemberSummariesRequest request,
        GetMemberSummariesQueryHandler query)
    {
        var members = await query.Execute(request);
        return Results.Ok(members);
    }

    public static async Task<IResult> GetMemberDetails(int id, GetMemberDetailsQueryHandler query)
    {
        var member = await query.Execute(id);
        return member is null ? Results.NotFound() : Results.Ok(member);
    }


    public static async Task<IResult> CreateMember(
        CreateMemberRequest request,
        CreateMemberCommandHandler handler)
    {
        try
        {
            var response = await handler.Execute(request);
            return Results.Created($"/members/{response.Id}", response);
        }
        catch (MemberEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }


    private static async Task<IResult> UpdateMember(
        int id,
        UpdateMemberRequest request,
        ClaimsPrincipal user,
        UpdateMemberCommandHandler handler)
    {
        if (!CanManageMember(user, id))
        {
            return Results.Forbid();
        }

        try
        {
            var updated = await handler.Execute(id, request);

            return !updated
                ? Results.NotFound()
                : Results.NoContent();
        }
        catch (MemberEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }


    private static async Task<IResult> DeleteMember(
        int id,
        ClaimsPrincipal user,
        DeleteMemberCommandHandler handler)
    {
        if (!CanManageMember(user, id))
        {
            return Results.Forbid();
        }

        try
        {
            var deleted = await handler.Execute(id);
            return !deleted
                ? Results.NotFound()
                : Results.NoContent();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static bool IsCurrentMember(ClaimsPrincipal user, int memberId)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var currentMemberId)
               && currentMemberId == memberId;
    }

    private static bool CanManageMember(
    ClaimsPrincipal user,
    int memberId)
    {
        if (user.IsInRole(nameof(MemberRole.Administrator)))
        {
            return true;
        }

        var claim =
            user.FindFirstValue(
                ClaimTypes.NameIdentifier);

        return int.TryParse(
                claim,
                out var currentMemberId)
            && currentMemberId == memberId;
    }
}