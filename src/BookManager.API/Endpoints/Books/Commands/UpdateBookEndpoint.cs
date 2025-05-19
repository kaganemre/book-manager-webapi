using BookManager.Application.Features.Books.Commands;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Commands;

public class UpdateBookEndpoint : Endpoint<UpdateBookCommandRequest, bool>
{
    private readonly UpdateBookCommandHandler _handler;
    public UpdateBookEndpoint(UpdateBookCommandHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Put("/api/books/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateBookCommandRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result.IsFailed)
        {
            await SendAsync(false, 404, ct);
            return;
        }

        await SendAsync(true, 200, ct);
    }
}