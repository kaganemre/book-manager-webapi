using BookManager.Application.Common.Services;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;

namespace BookManager.Application.Features.Auth.Commands;

public sealed record LoginUserCommand(string Email, string Password)
    : ICommand<LoginUserCommandResponse>;
public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty!")
            .EmailAddress().WithMessage("Please enter a valid email address!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty!")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
internal sealed class LoginUserCommandHandler(
    IIdentityService identityService,
    IJwtTokenService jwtTokenService
) : ICommandHandler<LoginUserCommand, LoginUserCommandResponse>
{
    public async Task<Result<LoginUserCommandResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var authenticatedUser = await identityService.ValidateCredentialsAsync(command.Email, command.Password);

        if (authenticatedUser is null)
        {
            return Result.Fail("Email or password is incorrect");
        }
        
        var token = jwtTokenService.GenerateToken(authenticatedUser);

        return Result.Ok(new LoginUserCommandResponse { Token = token });
    }
}
public sealed class LoginUserCommandResponse
{
    public string Token { get; set; } = default!;
}