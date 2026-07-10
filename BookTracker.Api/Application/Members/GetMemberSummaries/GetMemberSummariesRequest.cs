namespace BookTracker.Api.Application.GetMemberSummaries;

public class GetMemberSummeriessRequest
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
}