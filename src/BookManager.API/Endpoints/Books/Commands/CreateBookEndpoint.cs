using BookManager.Application.Features.Books.Commands;
using FastEndpoints;
using FluentResults;

namespace BookManager.API.Endpoints.Books.Commands;

public class CreateBookEndpoint : Endpoint<CreateBookCommandRequest, Result<CreateBookResponse>>
{
    private readonly CreateBookCommandHandler _handler;
    public CreateBookEndpoint(CreateBookCommandHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Post("/api/books");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateBookCommandRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result.IsFailed)
        {
            await SendAsync(result, 409, ct);
            return;
        }

        await SendAsync(result, 201, ct);
    }
}