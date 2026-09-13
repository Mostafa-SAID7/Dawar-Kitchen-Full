using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Services;
using NaarNoor.Infrastructure.Services;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService,
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Email and password</param>
    /// <returns>201 Created with userId</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/validation",
                Title = "Validation Error",
                Detail = "Email is required",
                Status = StatusCodes.Status400BadRequest
            });

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return BadRequest(new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/validation",
                Title = "Validation Error",
                Detail = "Password must be at least 8 characters",
                Status = StatusCodes.Status400BadRequest
            });

        try
        {
            var result = await _userService.RegisterAsync(request.Email, request.Password, "");
            if (!result.Success)
                return BadRequest(new ProblemDetails
                {
                    Type = "https://dawar-kitchen.api/errors/registration",
                    Title = "Registration Failed",
                    Detail = result.Error ?? "Registration failed",
                    Status = StatusCodes.Status400BadRequest
                });

            _logger.LogInformation("User registered: {Email}", request.Email);
            return Created("", new { userId = result.User!.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error for {Email}", request.Email);
            return StatusCode(500, new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/server",
                Title = "Internal Server Error",
                Detail = "Registration failed",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Email and password</param>
    /// <returns>200 OK with access_token (snake_case)</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/validation",
                Title = "Validation Error",
                Detail = "Email and password are required",
                Status = StatusCodes.Status400BadRequest
            });

        try
        {
            var result = await _userService.AuthenticateAsync(request.Email, request.Password);
            if (!result.Success)
            {
                _logger.LogWarning("Failed login attempt for {Email}", request.Email);
                return Unauthorized(new ProblemDetails
                {
                    Type = "https://dawar-kitchen.api/errors/authentication",
                    Title = "Authentication Failed",
                    Detail = "Invalid email or password",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            var token = _jwtService.GenerateToken(result.User!.Id, result.User!.Email, result.User!.Roles);
            _logger.LogInformation("User logged in: {Email}", request.Email);

            // Return EXACTLY { access_token: string } per spec (snake_case)
            return Ok(new { access_token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for {Email}", request.Email);
            return StatusCode(500, new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/server",
                Title = "Internal Server Error",
                Detail = "Login failed",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Logout the current user
    /// </summary>
    /// <returns>200 OK</returns>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("uid")?.Value;
        _logger.LogInformation("User logged out: {UserId}", userId);
        return Ok();
    }

    /// <summary>
    /// Reset password for a user (email-based)
    /// </summary>
    /// <param name="request">Email address</param>
    /// <returns>200 OK</returns>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] AuthResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/validation",
                Title = "Validation Error",
                Detail = "Email is required",
                Status = StatusCodes.Status400BadRequest
            });

        try
        {
            // Security: Don't reveal if email exists
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user != null)
            {
                _logger.LogInformation("Password reset requested for {Email}", request.Email);
                // TODO: Send reset email with token
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset error");
            return StatusCode(500, new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/server",
                Title = "Internal Server Error",
                Detail = "Password reset failed",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    /// <returns>200 OK with { userId, email }</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("uid")?.Value;
        var email = User.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/authentication",
                Title = "Unauthorized",
                Detail = "Invalid token",
                Status = StatusCodes.Status401Unauthorized
            });

        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
                return Unauthorized(new ProblemDetails
                {
                    Type = "https://dawar-kitchen.api/errors/authentication",
                    Title = "User Not Found",
                    Detail = "User not found",
                    Status = StatusCodes.Status401Unauthorized
                });

            // Return EXACTLY { userId, email } per spec
            return Ok(new { userId = user.Id, email = user.Email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get me error for {UserId}", userId);
            return StatusCode(500, new ProblemDetails
            {
                Type = "https://dawar-kitchen.api/errors/server",
                Title = "Internal Server Error",
                Detail = "Failed to get user",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}

/// <summary>
/// Request DTOs matching frontend contracts exactly
/// </summary>
public class AuthRegisterRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class AuthLoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class AuthResetPasswordRequest
{
    public string Email { get; set; } = "";
}
