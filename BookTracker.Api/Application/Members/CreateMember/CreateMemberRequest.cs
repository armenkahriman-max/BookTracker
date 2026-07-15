namespace BookTracker.Api.Application.CreateMember;

public class CreateMemberRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

}