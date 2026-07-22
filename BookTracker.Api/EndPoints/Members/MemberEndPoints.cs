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
        .RequireAuthorization();

        app.MapGet("/members/{id:int}", GetMemberDetails)
        .RequireAuthorization();

        app.MapPost("/members", CreateMember);

        app.MapPut("/members/{id:int}", UpdateMember)
            .RequireAuthorization();

        app.MapDelete("/members/{id:int}", DeleteMember)
            .RequireAuthorization();

        return app;
    }


    private static async Task<IResult> GetMemberSummaries(
     [AsParameters]
    GetMemberSummariesRequest request,
     ClaimsPrincipal principal,
     GetMemberSummariesQueryHandler handler)
    {
        try
        {
            var actor =
                principal.ToActor();

            var response =
                await handler.Execute(
                    actor,
                    request);

            return Results.Ok(response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> GetMemberDetails(
        int id,
        ClaimsPrincipal principal,
        GetMemberDetailsQueryHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var response = await handler.Execute(actor, id);

            return response is null ? Results.NotFound() : Results.Ok(response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
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
        ClaimsPrincipal principal,
        UpdateMemberCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var updated = await handler.Execute(actor, id, request);

            return updated ? Results.NoContent() : Results.NotFound();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
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
        ClaimsPrincipal principal,
        DeleteMemberCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var deleted = await handler.Execute(actor, id);

            return deleted ? Results.NoContent() : Results.NotFound();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
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