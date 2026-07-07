using BookManager.Application.Common.Decorators.Logging;
using BookManager.Application.Common.Decorators.Validation;
using BookManager.Application.Features.Books.Commands;
using BookManager.Application.Interfaces.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookManager.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssemblyOf<CreateBookCommandHandler>()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.Scan(scan => scan.FromAssemblyOf<CreateBookCommandValidator>()
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingCommandBaseHandler<>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationCommandBaseHandler<>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandler<,>));

        return services;
    }
}