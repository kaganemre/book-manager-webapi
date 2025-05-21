using BookManager.Application.Features.Books.Queries;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetAllBooksEndpoint : EndpointWithoutRequest<IReadOnlyList<GetAllBooksQueryResponse>>
{
    private readonly GetAllBooksQueryHandler _handler;
    public GetAllBooksEndpoint(GetAllBooksQueryHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Get("/books");
        Roles("Admin", "User");
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(ct);
        await SendOkAsync(result, ct);
    }
}