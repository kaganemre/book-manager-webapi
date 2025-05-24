using BookManager.Application.Features.Books.Commands;
using Messaging = BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Commands;

public class DeleteBookEndpoint(Messaging.ICommandHandler<DeleteBookCommand> handler) : Endpoint<DeleteBookCommand>
{
    public override void Configure()
    {
        Delete("/books/{id}");
        Roles("Admin");
    }
    public override async Task HandleAsync(DeleteBookCommand req, CancellationToken ct)
    {
        await handler.Handle(req, ct);
        await SendNoContentAsync(ct);
    }
}