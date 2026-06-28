using BookManager.API.Endpoints.Books.Queries;
using BookManager.Application.Features.Books.Commands;
using BookManager.Application.Interfaces.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.API.Endpoints.Books.Commands;

public static class CreateBookEndpoint
{
    public static void MapCreateBook(this IEndpointRouteBuilder app)
    {
        app.MapPost("/books", async (
                [FromBody] CreateBookCommand command,
                ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(command, ct);

                if (result.IsFailed)
                {
                    return Results.Conflict(result.Errors.Select(e => e.Message));
                }

                return Results.CreatedAtRoute(
                    GetBookByIdEndpoint.Name,
                    new { id = result.Value.Id },
                    result.Value);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}