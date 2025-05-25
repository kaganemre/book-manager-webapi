using BookManager.Application.Features.Books.Queries;
using BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetAllBooksEndpoint(IQueryHandler<GetAllBooksQuery, IReadOnlyList<GetAllBooksQueryResponse>> handler)
    : EndpointWithoutRequest<IReadOnlyList<GetAllBooksQueryResponse>>
{
    public override void Configure()
    {
        Get("/books");
        Roles("Admin", "User");
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await handler.Handle(new GetAllBooksQuery(), ct);
        await SendOkAsync(result.Value, ct);
    }
}