namespace BookTracker.Api.Domain;

public record MemberEmail
{
    public const int MaxLength = 200;

    public string Value { get; }

    public MemberEmail(string value)
    {

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email is required.");
        }

        var cleaned = value.Trim().ToLowerInvariant();

        if (!cleaned.Contains("@"))
        {
            throw new DomainException("Email must contain an @ symbol.");
        }
        

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Email cannot contain more than {MaxLength} characters.");
        }

        Value = cleaned;
    }

    public static implicit operator string(MemberEmail email)
    {
        return email.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}