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
        Post("/api/books");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteBookCommandRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result.IsFailed)
        {
            await SendAsync(false, 404, ct);
        }

        await SendAsync(true, 204, ct);
    }
}