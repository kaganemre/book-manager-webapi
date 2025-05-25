using BookManager.Application.Features.Auth.Commands;
using Messaging = BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Auth.Commands;

public class LoginUserEndpoint(Messaging.ICommandHandler<LoginUserCommand, LoginUserCommandResponse> handler)
    : Endpoint<LoginUserCommand, LoginUserCommandResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }
    public override async Task HandleAsync(LoginUserCommand req, CancellationToken ct)
    {
        var result = await handler.Handle(req, ct);

        if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);

            ThrowIfAnyErrors(401);
        }

        await SendOkAsync(result.Value, ct);
    }

}