namespace BookTracker.Api.Application.Members.GetMemberSummaries;
public class MemberSummary
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}