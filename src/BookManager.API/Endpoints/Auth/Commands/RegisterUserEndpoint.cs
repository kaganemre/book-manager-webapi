using BookManager.Application.Features.Auth.Commands;
using BookManager.Application.Interfaces.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.API.Endpoints.Auth.Commands;

public static class RegisterUserEndpoint
{
    public static void MapRegisterUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (
            [FromBody] RegisterUserCommand command,
            ICommandHandler<RegisterUserCommand> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(command, ct);

            if (result.IsFailed)
            {
                return Results.BadRequest(result.Errors.Select(e => e.Message));
            }

            return Results.Created();
        })
        .AllowAnonymous();
    }
}