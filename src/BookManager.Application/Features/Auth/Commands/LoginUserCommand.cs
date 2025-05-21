using BookManager.Application.Common.Services;
using FastEndpoints;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace BookManager.Application.Features.Auth.Commands;

public sealed record LoginUserCommandRequest(string Email, string Password);
public sealed class LoginUserCommandRequestValidator : Validator<LoginUserCommandRequest>
{
    public LoginUserCommandRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-post boş olamaz!")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz!");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Şifre boş olamaz!")
        .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır!");
    }
}
public sealed class LoginUserCommandHandler
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    public LoginUserCommandHandler(UserManager<IdentityUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<LoginUserCommandResponse> HandleAsync(LoginUserCommandRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, req.Password))
        {
            throw new UnauthorizedAccessException("Geçersiz kullanıcı adı veya şifre!");
        }

        return new LoginUserCommandResponse { Token = await _jwtTokenService.GenerateToken(user) };
    }
}
public sealed class LoginUserCommandResponse
{
    public string Token { get; set; } = default!;
}