using System.ComponentModel.DataAnnotations;

namespace NaarNoor.API.DTOs;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(8)]
    public string Password { get; set; } = "";

    [Required]
    public string FullName { get; set; } = "";
}

public class RefreshTokenRequest
{
    public string? RefreshToken { get; set; }
}
