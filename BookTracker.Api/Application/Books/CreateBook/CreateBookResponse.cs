namespace BookTracker.Api.Application.CreateBook;

public class CreateBookResponse //DTOS = 
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public int Year { get; set; }
}