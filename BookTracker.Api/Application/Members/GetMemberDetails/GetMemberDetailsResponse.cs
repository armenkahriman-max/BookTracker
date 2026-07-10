namespace BookTracker.Api.Application.Members.GetMemberDetailsResponse;
public class GetMemberDetailsResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}