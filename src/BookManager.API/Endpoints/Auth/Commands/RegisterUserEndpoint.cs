using BookManager.Application.Features.Auth.Commands;
using FastEndpoints;

namespace BookManager.API.Endpoints.Auth.Commands;

public class RegisterUserEndpoint : Endpoint<RegisterUserCommandRequest, bool>
{
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserEndpoint(RegisterUserCommandHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }
    public override async Task HandleAsync(RegisterUserCommandRequest req, CancellationToken ct)
    {
        await _handler.HandleAsync(req, ct);
        await SendAsync(true, 201, ct);
    }
}