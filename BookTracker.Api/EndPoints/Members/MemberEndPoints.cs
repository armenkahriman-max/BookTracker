using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Application.DeleteMember;
using BookTracker.Api.Application.GetMemberDetails;
using BookTracker.Api.Application.GetMemberSummaries;
using BookTracker.Api.Application.GetMemberSummariesQueryHandler;
using BookTracker.Api.Application.UpdateMember;
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
        var members = await handler.Execute(principal.ToActor(), request);
        return Results.Ok(members);
    }

    private static async Task<IResult> GetMemberDetails(
        int id,
        ClaimsPrincipal principal,
        GetMemberDetailsQueryHandler handler)
    {
        var response = await handler.Execute(principal.ToActor(), id);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }


    public static async Task<IResult> CreateMember(
        CreateMemberRequest request,
        CreateMemberCommandHandler handler)
    {
        var member = await handler.Execute(request);
        return Results.Created($"/members/{member.Id}", member);
    }


    private static async Task<IResult> UpdateMember(
        int id,
        UpdateMemberRequest request,
        ClaimsPrincipal principal,
        UpdateMemberCommandHandler handler)
    {
        var updated = await handler.Execute(principal.ToActor(), id, request);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> DeleteMember(
        int id,
        ClaimsPrincipal principal,
        DeleteMemberCommandHandler handler)
    {
        var deleted = await handler.Execute(principal.ToActor(), id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

}