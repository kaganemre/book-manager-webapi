using BookManager.Application.Features.Auth.Commands;
using BookManager.Application.Features.Books.Commands;
using BookManager.Application.Features.Books.Queries;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.Extensions.DependencyInjection;

namespace BookManager.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetAllBooksQueryHandler>();
        services.AddScoped<GetBookByIdQueryHandler>();
        services.AddScoped<CreateBookCommandHandler>();
        services.AddScoped<UpdateBookCommandHandler>();
        services.AddScoped<DeleteBookCommandHandler>();
        services.AddScoped<LoginUserCommandHandler>();
        services.AddScoped<RegisterUserCommandHandler>();

        services.AddFastEndpoints();

        return services;
    }
}