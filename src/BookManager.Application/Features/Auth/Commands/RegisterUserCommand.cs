using BookManager.Application.Common.Exceptions;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BookManager.Application.Features.Auth.Commands;

public sealed record RegisterUserCommandRequest(string UserName, string Email, string Password);
public sealed class RegisterUserCommandRequestValidator : Validator<RegisterUserCommandRequest>
{
    public RegisterUserCommandRequestValidator()
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
public sealed class RegisterUserCommandHandler
{
    private readonly UserManager<IdentityUser> _userManager;
    public RegisterUserCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task HandleAsync(RegisterUserCommandRequest req, CancellationToken ct)
    {
        var user = new IdentityUser
        {
            UserName = req.UserName,
            Email = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            throw new ConflictException("Bu kullanıcı adı veya e-posta zaten kayıtlı!");
        }
    }
}
