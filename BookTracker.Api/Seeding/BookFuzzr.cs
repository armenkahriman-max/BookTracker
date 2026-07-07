using BookTracker.Api.Domain;
using QuickFuzzr;

namespace BookTracker.Api.Seeding;

public static class BookFuzzr
{
    public static IEnumerable<Book> Many(int count)
        => One.Many(count).Generate();

    private static readonly string[] Adjectives =
    [
        "Suspicious",
        "Melancholy",
        "Quantum",
        "Reluctant",
        "Extremely Polite",
        "Mildly Haunted",
        "Unreasonably Confident",
        "Invisible",
        "Chronically Late",
        "Over-Caffeinated"
    ];

    private static readonly string[] Nouns =
    [
        "Badger",
        "Librarian",
        "Spaceship",
        "Cupcake",
        "Philosopher",
        "Typewriter",
        "Goblin",
        "Umbrella",
        "Database",
        "Octopus"
    ];

    private static readonly string[] Situations =
    [
        "Who Knew Too Much",
        "At the End of Time",
        "With a Suspicious Hat",
        "In Production",
        "Under the Stairs",
        "Against Better Judgement",
        "During Standup",
        "With No Unit Tests",
        "On a Tuesday",
        "After the Refactor"
    ];

    private static readonly string[] FirstNames =
    [
        "Ada",
        "Grace",
        "Douglas",
        "Ursula",
        "Terry",
        "Octavia",
        "Isaac",
        "Mary",
        "Kurt",
        "Agatha"
    ];

    private static readonly string[] LastNames =
    [
        "Byte",
        "Stackwell",
        "Nullman",
        "Loopington",
        "Brackets",
        "Mergefield",
        "Bugworthy",
        "Semicolon",
        "Heap",
        "Async"
    ];

    private static readonly FuzzrOf<string> Situational =
        from adjective in Fuzzr.OneOf(Adjectives)
        from noun in Fuzzr.OneOf(Nouns)
        from situation in Fuzzr.OneOf(Situations)
        select $"The {adjective} {noun} {situation}";

    private static readonly FuzzrOf<string> Memoir =
        from adjective in Fuzzr.OneOf(Adjectives)
        from noun in Fuzzr.OneOf(Nouns)
        select $"My Life as an {adjective} {noun}";

    private static readonly FuzzrOf<string> Academic =
        from adjective in Fuzzr.OneOf(Adjectives)
        from noun in Fuzzr.OneOf(Nouns)
        select $"A Brief History of {adjective} {noun}s";

    private static readonly FuzzrOf<string> Title =
        Fuzzr.OneOf(Situational, Memoir, Academic);

    private static readonly FuzzrOf<string> Author =
        from firstName in Fuzzr.OneOf(FirstNames)
        from lastName in Fuzzr.OneOf(LastNames)
        select $"{firstName} {lastName}";

    private static readonly FuzzrOf<Book> One =
        from title in Title
        from author in Author
        from year in Fuzzr.Int(1930, 2026)
        select new Book
        {
            Title = new BookTitle(title),
            Author = new AuthorName(author),
            Year = year
        };
}