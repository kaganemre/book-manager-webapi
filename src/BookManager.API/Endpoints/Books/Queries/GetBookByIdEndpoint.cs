using BookManager.Application.Features.Books.Queries;
using BookManager.Application.Interfaces.Messaging;
using FastEndpoints;

namespace BookManager.API.Endpoints.Books.Queries;

public static class GetBookByIdEndpoint
{
    public const string Name = "GetBookById";

    public static void MapGetBookById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/books/{id:guid}", async (
                Guid id,
                IQueryHandler<GetBookByIdQuery, GetBookByIdQueryResponse> handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(new GetBookByIdQuery(id), ct);

                if (result.IsFailed)
                {
                    return Results.NotFound(result.Errors.Select(e => e.Message));
                }

                return Results.Ok(result.Value);
            })
            .WithName(Name)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));
    }
}