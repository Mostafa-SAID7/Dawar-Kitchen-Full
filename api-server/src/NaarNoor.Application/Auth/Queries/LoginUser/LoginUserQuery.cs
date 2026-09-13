using MediatR;

namespace NaarNoor.Application.Auth.Queries.LoginUser;

/// <summary>
/// Query to authenticate user and get JWT token
/// </summary>
public record LoginUserQuery(
    string Email,
    string Password
) : IRequest<LoginUserResult>;

/// <summary>
/// Result of user login
/// </summary>
public record LoginUserResult(
    string UserId,
    string Email,
    string AccessToken
);
