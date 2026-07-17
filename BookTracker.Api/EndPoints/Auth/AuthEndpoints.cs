using System.Security.Claims;
using BookTracker.Api.Application.Auth.GetCurrentMember;
using BookTracker.Api.Application.Auth.Login;


namespace BookTracker.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", Login);

        app.MapGet("/auth/me", GetCurrentMember)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        LoginCommandHandler handler)
    {
        var response = await handler.Execute(request);

        if (response is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(response);
    }

    private static IResult GetCurrentMember(
        ClaimsPrincipal user)
    {
        var id =
            user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var name =
            user.FindFirst(ClaimTypes.Name)!.Value;

        var email =
            user.FindFirst(ClaimTypes.Email)!.Value;

        var role =
            user.FindFirst(ClaimTypes.Role)!.Value;

        return
            Results.Ok(
                new CurrentMemberResponse
                {
                    Id = int.Parse(id),
                    Name = name,
                    Email = email,
                    Role = role
                });
    }
}