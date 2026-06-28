using BookManager.Application.Features.Books.Commands;
using BookManager.Application.Interfaces.Messaging;


namespace BookManager.API.Endpoints.Books.Commands;

public static class DeleteBookEndpoint
{
    public static void MapDeleteBook(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/books/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteBookCommand> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteBookCommand(id), ct);

            if (result.IsFailed)
            {
                return Results.NotFound(result.Errors.Select(e => e.Message));
            }

            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}