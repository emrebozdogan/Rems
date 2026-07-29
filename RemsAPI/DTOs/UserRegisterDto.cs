using System.ComponentModel.DataAnnotations;

namespace RemsAPI.DTOs;

public class UserRegisterDto
{

  [Required(ErrorMessage = "Name is required")]
  public string Name { get; set; } = "";

  [Required(ErrorMessage = "Email is required.")]
  [EmailAddress(ErrorMessage = "Invalid email format.")]
  public string Email { get; set; } = "";

  [Required(ErrorMessage = "Password is required.")]
  [StringLength(12, ErrorMessage = "The password must be at least 8 characters long.", MinimumLength = 8)]
  [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&.#',])[A-Za-z\d@$!%*?&.#',]{8,}$", ErrorMessage = "Password must include at least one letter, one number, and one special character.")]
  public string Password { get; set; } = "";
}
