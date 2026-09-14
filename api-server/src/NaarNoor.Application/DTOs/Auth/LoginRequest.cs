using System.ComponentModel.DataAnnotations;

namespace NaarNoor.Application.DTOs.Auth;

/// <summary>
/// Request DTO for user login.
/// ✅ Moved from API layer (AuthController.cs) to Application layer
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = "";
}
