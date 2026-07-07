namespace BookTracker.Api.Application.BookList;

public class GetBookListRequest
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public string? Search { get; set; }
}