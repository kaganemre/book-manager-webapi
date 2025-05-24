using BookManager.API.Endpoints.Books.Queries;
using BookManager.Application.Features.Books.Commands;
using Messaging = BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Commands;

public class CreateBookEndpoint(Messaging.ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler) : Endpoint<CreateBookCommand, CreateBookCommandResponse>
{
    public override void Configure()
    {
        Post("/books");
        Roles("Admin");
    }
    public override async Task HandleAsync(CreateBookCommand req, CancellationToken ct)
    {
        var result = await handler.Handle(req, ct);

        await SendCreatedAtAsync<GetBookByIdEndpoint>(
            new { id = result.Value.Id },
            result.Value,
            cancellation: ct
        );
    }
}