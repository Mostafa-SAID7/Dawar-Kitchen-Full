using MediatR;

namespace NaarNoor.Application.Features.Auth.Commands.RegisterUser;

/// <summary>
/// Command to register a new user
/// </summary>
public record RegisterUserCommand(
    string Email,
    string Password,
    string FullName
) : IRequest<RegisterUserResult>;

/// <summary>
/// Result of user registration
/// </summary>
public record RegisterUserResult(
    string UserId,
    string Email,
    bool Success
);

