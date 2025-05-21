using BookManager.Application.Features.Books.Queries;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetBookByIdEndpoint : Endpoint<GetBookByIdQueryRequest, GetBookByIdQueryResponse>
{
    private readonly GetBookByIdQueryHandler _handler;
    public GetBookByIdEndpoint(GetBookByIdQueryHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Get("/book/{id}");
        Roles("User");
    }
    public override async Task HandleAsync(GetBookByIdQueryRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);
        await SendOkAsync(result, ct);
    }
}