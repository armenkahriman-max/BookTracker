namespace BookTracker.Api.Application.Auth.GetCurrentMember;

public class CurrentMemberResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }
}