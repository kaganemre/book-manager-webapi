using BookManager.Application.Features.Books.Queries;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetAllBooksEndpoint : EndpointWithoutRequest<IReadOnlyList<GetAllBooksResponse>>
{
    private readonly GetAllBooksQueryHandler _handler;
    public GetAllBooksEndpoint(GetAllBooksQueryHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Get("/api/books");
        AllowAnonymous();
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(ct);
        await SendAsync(result, cancellation: ct);
    }
}