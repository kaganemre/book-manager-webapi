using BookManager.Application.Common.Services;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BookManager.Application.Features.Auth.Commands;

public sealed record LoginUserCommand(string Email, string Password)
    : ICommand<LoginUserCommandResponse>;
public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-post boş olamaz!")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz!");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Şifre boş olamaz!")
        .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır!");
    }
}
internal sealed class LoginUserCommandHandler(
    UserManager<IdentityUser> userManager,
    IJwtTokenService jwtTokenService
) : ICommandHandler<LoginUserCommand, LoginUserCommandResponse>
{
    public async Task<Result<LoginUserCommandResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            return Result.Fail("Kullanıcı bulunamadı.");
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
        {
            return Result.Fail("Geçersiz kullanıcı adı veya şifre.");
        }

        var token = await jwtTokenService.GenerateToken(user);

        return Result.Ok(new LoginUserCommandResponse { Token = token });
    }
}
public sealed class LoginUserCommandResponse
{
    public string Token { get; set; } = default!;
}