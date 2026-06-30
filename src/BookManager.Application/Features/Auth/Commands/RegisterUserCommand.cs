using BookManager.Application.Common.Models;
using BookManager.Application.Common.Services;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;

namespace BookManager.Application.Features.Auth.Commands;

public sealed record RegisterUserCommand(string UserName, string Email, string Password)
    : ICommand;
public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username cannot be empty!")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty!")
            .EmailAddress().WithMessage("Please enter a valid email address!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty!")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
internal sealed class RegisterUserCommandHandler(IIdentityService identityService)
    : ICommandHandler<RegisterUserCommand>
{
    public async Task<Result> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var request = new CreateUserRequest(command.UserName, command.Email, command.Password);
        
        var result = await identityService.CreateAsync(request);

        if (result.IsFailed)
        {
            return result;
        }

        return Result.Ok();
    }
}
