using BookManager.API.Endpoints.Books.Queries;
using BookManager.Application.Features.Books.Commands;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Commands;

public class CreateBookEndpoint : Endpoint<CreateBookCommandRequest, CreateBookCommandResponse>
{
    private readonly CreateBookCommandHandler _handler;
    public CreateBookEndpoint(CreateBookCommandHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Post("/books");
        Roles("Admin");
    }
    public override async Task HandleAsync(CreateBookCommandRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        await SendCreatedAtAsync<GetBookByIdEndpoint>(
            new { id = result.Id },
            result,
            cancellation: ct
        );
    }
}