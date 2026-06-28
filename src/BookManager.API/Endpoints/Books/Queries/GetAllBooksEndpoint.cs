using BookManager.Application.Features.Books.Queries;
using BookManager.Application.Interfaces.Messaging;

namespace BookManager.API.Endpoints.Books.Queries;

public static class GetAllBooksEndpoint
{
    public static void MapGetAllBooks(this IEndpointRouteBuilder app)
    {
        app.MapGet("/books", async (
                IQueryHandler<GetAllBooksQuery, IReadOnlyList<GetAllBooksQueryResponse>> handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(new GetAllBooksQuery(), ct);
                return Results.Ok(result.Value);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));
    }
}