using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

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
internal sealed class RegisterUserCommandHandler(UserManager<IdentityUser> userManager)
    : ICommandHandler<RegisterUserCommand>
{
    public async Task<Result> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = new IdentityUser
        {
            UserName = command.UserName,
            Email = command.Email
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return Result.Fail(errors);
        }

        return Result.Ok();
    }
}
