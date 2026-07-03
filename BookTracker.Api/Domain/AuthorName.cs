namespace BookTracker.Api.Domain;

public sealed record AuthorName
{
    public const int MaxLength = 100;

    public string Value { get; }

    public AuthorName(string value)
    {
        var cleaned = value.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Author is required.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Author cannot be longer than {MaxLength} characters.");
        }

        Value = cleaned;
    }

    public override string ToString()
    {
        return Value;
    }
}