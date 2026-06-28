using BookManager.API.Endpoints.Auth.Commands;

namespace BookManager.API.Extensions;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapLoginUser();
        app.MapRegisterUser();
    }
}