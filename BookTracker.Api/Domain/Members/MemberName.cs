namespace BookTracker.Api.Domain;

public sealed record MemberName
{
    public const int MaxLength = 100;

    public string Value { get; }

    public MemberName(string value)
    {
        var cleaned = value.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Member name is required.");
        }
        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Member name cannot be longer then {MaxLength} characters.");
        }
        Value = cleaned;
    }

    public static implicit operator string(MemberName member)
    {
        return member.Value;
    }
    public override string ToString()
    {
        return Value;
    }
}