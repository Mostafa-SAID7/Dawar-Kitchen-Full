using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.Services;

namespace NaarNoor.Application.Features.Auth.Queries.LoginUser;

/// <summary>
/// Handler for LoginUserQuery
/// Authenticates user and generates JWT token
/// </summary>
public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginUserResult>
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public LoginUserQueryHandler(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.AuthenticateAsync(
            request.Email,
            request.Password);

        if (!user.Success || user.User is null)
            throw new UnauthorizedAccessException(user.Error ?? "Invalid email or password.");

        var token = _jwtService.GenerateToken(user.User.Id, user.User.Email, user.User.Roles);

        return new LoginUserResult(
            user.User.Id,
            user.User.Email,
            token);
    }
}

