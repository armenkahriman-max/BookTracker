namespace BookTracker.Api.Application.CreateMember;

public class CreateMemberResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }

}