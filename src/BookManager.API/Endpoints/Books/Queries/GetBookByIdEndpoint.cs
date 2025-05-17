using BookManager.Application.Features.Books.Queries;
using FastEndpoints;
using FluentResults;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetBookByIdEndpoint : Endpoint<GetBookByIdRequest, Result<GetBookByIdResponse>>
{
    private readonly GetBookByIdQueryHandler _handler;
    public GetBookByIdEndpoint(GetBookByIdQueryHandler handler)
    {
        _handler = handler;
    }
    public override void Configure()
    {
        Get("/api/book/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetBookByIdRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result.IsFailed)
        {
            await SendAsync(result, 404, ct);
            return;
        }

        await SendAsync(result, 200, ct);
    }
}