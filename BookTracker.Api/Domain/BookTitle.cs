namespace BookTracker.Api.Domain;

public sealed record BookTitle
{
    public const int MaxLength = 100;

    public string Value { get; }

    public BookTitle(string value)
    {
        var cleaned = value.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Title is required.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Title cannot be longer than {MaxLength} characters.");
        }

        Value = cleaned;
    }

    public override string ToString()
    {
        return Value;
    }
}