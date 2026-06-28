using BookManager.Application.Features.Auth.Commands;
using BookManager.Application.Interfaces.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.API.Endpoints.Auth.Commands;

public static class LoginUserEndpoint
{
    public static void MapLoginUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            [FromBody] LoginUserCommand command,
            ICommandHandler<LoginUserCommand, LoginUserCommandResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(command, ct);

            if (result.IsFailed)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(result.Value);
        })
        .AllowAnonymous();
    }
}