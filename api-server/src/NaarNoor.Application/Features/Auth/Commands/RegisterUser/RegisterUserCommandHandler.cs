using MediatR;
using NaarNoor.Application.Services;

namespace NaarNoor.Application.Auth.Commands.RegisterUser;

/// <summary>
/// Handler for RegisterUserCommand
/// Delegates to IUserService for actual registration logic
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserService _userService;

    public RegisterUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName);

        if (!result.Success)
            throw new InvalidOperationException(result.Error ?? "Registration failed.");

        return new RegisterUserResult(
            result.User!.Id,
            result.User.Email,
            true);
    }
}
