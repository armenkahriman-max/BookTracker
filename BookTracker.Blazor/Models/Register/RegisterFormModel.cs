using System.ComponentModel.DataAnnotations;

namespace BookTracker.Blazor.Models.Register;

public class RegisterFormModel
{
    [Required][MaxLength(100)]
    public string Name { get; set; } = "";
   [Required][MaxLength(200)]
    public string Email { get; set; } = "";
    [Required][MinLength(8)]
    public string Password  { get; set; } = "";
    [Required]
    [Compare(nameof(Password))]
    public string PasswordConfirmation { get; set; } = "";
}