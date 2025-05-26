using BookManager.Application.Common.Decorators.Logging;
using BookManager.Application.Common.Decorators.Validation;
using BookManager.Application.Features.Books.Commands;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Messaging = BookManager.Application.Interfaces.Messaging;

namespace BookManager.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssemblyOf<CreateBookCommandHandler>()
            .AddClasses(classes => classes.AssignableTo(typeof(Messaging.ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(Messaging.ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(Messaging.IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.Scan(scan => scan.FromAssemblyOf<CreateBookCommandValidator>()
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.Decorate(typeof(Messaging.ICommandHandler<,>), typeof(LoggingCommandHandler<,>));
        services.Decorate(typeof(Messaging.ICommandHandler<>), typeof(LoggingCommandBaseHandler<>));
        services.Decorate(typeof(Messaging.ICommandHandler<,>), typeof(ValidationCommandHandler<,>));
        services.Decorate(typeof(Messaging.ICommandHandler<>), typeof(ValidationCommandBaseHandler<>));

        services.AddFastEndpoints();

        return services;
    }
}