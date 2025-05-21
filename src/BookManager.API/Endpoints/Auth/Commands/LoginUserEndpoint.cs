using BookManager.Application.Features.Auth.Commands;
using FastEndpoints;

namespace BookManager.API.Endpoints.Auth.Commands;

public class LoginUserEndpoint : Endpoint<LoginUserCommandRequest, LoginUserCommandResponse>
{
    private readonly LoginUserCommandHandler _handler;
    public LoginUserEndpoint(LoginUserCommandHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }
    public override async Task HandleAsync(LoginUserCommandRequest req, CancellationToken ct)
    {
        await SendOkAsync(await _handler.HandleAsync(req, ct), ct);
    }

}