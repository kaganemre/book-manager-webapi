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
        Get("/api/book/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetBookByIdQueryRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result is null)
        {
            ThrowError("Kitap bulunamadı.", 404);
        }

        await SendAsync(result, 200, ct);
    }
}