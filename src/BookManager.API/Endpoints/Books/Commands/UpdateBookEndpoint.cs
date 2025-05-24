using BookManager.Application.Features.Books.Commands;
using Messaging = BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Commands;

public class UpdateBookEndpoint(Messaging.ICommandHandler<UpdateBookCommand> handler)
    : Endpoint<UpdateBookCommand>
{
    public override void Configure()
    {
        Put("/books/{id}");
        Roles("Admin");
    }
    public override async Task HandleAsync(UpdateBookCommand req, CancellationToken ct)
    {
        await handler.Handle(req, ct);
        await SendNoContentAsync(ct);
    }
}