using BookManager.Application.Features.Books.Queries;
using BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetBookByIdEndpoint(IQueryHandler<GetBookByIdQuery, GetBookByIdQueryResponse> handler)
    : Endpoint<GetBookByIdQuery, GetBookByIdQueryResponse>
{
    public override void Configure()
    {
        Get("/book/{id}");
        Roles("Admin", "User");
    }
    public override async Task HandleAsync(GetBookByIdQuery req, CancellationToken ct)
    {
        var result = await handler.Handle(req, ct);

        if (result.IsFailed)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(result.Value, ct);
    }
}