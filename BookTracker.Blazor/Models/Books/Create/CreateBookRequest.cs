
using System.ComponentModel.DataAnnotations;

namespace BookTracker.Blazor.Models.Books.Create;

public class CreateBookRequest
{
    [Required(ErrorMessage = "Title is required")]
    public  string Title { get; set; } = "";
    [Required(ErrorMessage = "Author is required")]
    public  string Author { get; set; } = "";
    [Range(minimum: 1000, maximum: 2026, ErrorMessage = "Year can only be between 1000 and 2026")]
    public int Year { get; set; }

}