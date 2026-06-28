using BookManager.API.Endpoints.Books.Commands;
using BookManager.API.Endpoints.Books.Queries;

namespace BookManager.API.Extensions;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateBook();
        app.MapGetBookById();
    }
}