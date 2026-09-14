using System.ComponentModel.DataAnnotations;

namespace NaarNoor.Application.DTOs.Auth;

/// <summary>
/// Request DTO for user registration.
/// ✅ Moved from API layer (AuthController.cs) to Application layer
/// ✅ Consolidates duplicate definitions (was AuthRegisterRequest + RegisterRequest)
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Full name is required")]
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Full name must not exceed 100 characters")]
    public string FullName { get; set; } = "";
}
