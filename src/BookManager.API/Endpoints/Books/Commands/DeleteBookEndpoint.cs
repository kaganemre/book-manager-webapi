using BookManager.Application.Features.Books.Commands;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Commands;

public class DeleteBookEndpoint : Endpoint<DeleteBookCommandRequest, bool>
{
    private readonly DeleteBookCommandHandler _handler;
    public DeleteBookEndpoint(DeleteBookCommandHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Delete("/api/books/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteBookCommandRequest req, CancellationToken ct)
    {
        await _handler.HandleAsync(req, ct);
        await SendNoContentAsync(ct);
    }
}