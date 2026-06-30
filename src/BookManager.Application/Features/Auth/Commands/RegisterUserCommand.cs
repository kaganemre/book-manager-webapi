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
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz!")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır");

        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("E-posta boş olamaz!")
           .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz!")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır!");
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
