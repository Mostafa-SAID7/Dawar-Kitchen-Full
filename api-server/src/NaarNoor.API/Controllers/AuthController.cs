using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Features.Auth.Commands.RegisterUser;
using NaarNoor.Application.Features.Auth.Queries.LoginUser;

namespace NaarNoor.API.Controllers;

/// <summary>
/// Authentication endpoints for user registration and login
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Email, password, and full name</param>
    /// <returns>201 Created with userId</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] AuthRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FullName ?? "");

        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("User registered: {Email}", request.Email);
        return Created("", new { userId = result.UserId });
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Email and password</param>
    /// <returns>200 OK with access_token and user details</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] AuthLoginRequest request,
        CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.Email, request.Password);
        var result = await _mediator.Send(query, cancellationToken);

        _logger.LogInformation("User logged in: {Email}", request.Email);
        return Ok(new
        {
            access_token = result.AccessToken,
            user_id = result.UserId,
            email = result.Email
        });
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    /// <returns>200 OK with user details</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetMe()
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("uid")?.Value;
        var email = User.FindFirst("email")?.Value;

        return Ok(new { userId, email });
    }
}

/// <summary>
/// Request DTO for user registration
/// </summary>
public class AuthRegisterRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// User password (min 8 chars)
    /// </summary>
    public string Password { get; set; } = "";

    /// <summary>
    /// User full name
    /// </summary>
    public string? FullName { get; set; }
}

/// <summary>
/// Request DTO for user login
/// </summary>
public class AuthLoginRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// User password
    /// </summary>
    public string Password { get; set; } = "";
}

