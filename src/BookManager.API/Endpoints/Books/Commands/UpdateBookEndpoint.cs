using BookManager.Application.Features.Books.Commands;
using BookManager.Application.Interfaces.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.API.Endpoints.Books.Commands;

public static class UpdateBookEndpoint
{
    public static void MapUpdateBook(this IEndpointRouteBuilder app)
    {
        app.MapPut("/books/{id:guid}", async (
                Guid id,
                [FromBody] UpdateBookCommand command,
                ICommandHandler<UpdateBookCommand> handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(command with { Id = id }, ct);

                if (result.IsFailed)
                {
                    return Results.NotFound(result.Errors.Select(e => e.Message));
                }

                return Results.NoContent();
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}