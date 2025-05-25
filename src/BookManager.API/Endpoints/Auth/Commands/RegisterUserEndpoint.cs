using BookManager.Application.Features.Auth.Commands;
using Messaging = BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Auth.Commands;

public class RegisterUserEndpoint(Messaging.ICommandHandler<RegisterUserCommand> handler)
    : Endpoint<RegisterUserCommand>
{
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }
    public override async Task HandleAsync(RegisterUserCommand req, CancellationToken ct)
    {
        var result = await handler.Handle(req, ct);

        if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);
            ThrowIfAnyErrors(400);
        }

        await SendAsync(true, 201, ct);
    }
}