# 📚 Book Manager

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-success)
![CQRS](https://img.shields.io/badge/CQRS-Decorator%20Pattern-blueviolet)
![License](https://img.shields.io/github/license/kaganemre/book-manager-webapi)

After MediatR — which has almost become synonymous with the CQRS Pattern — announced its transition to a commercial license, the CQRS approach implemented with the Decorator Pattern (popularized by Milan Jovanović) has stood out among free alternatives. Its flexible architecture, clear separation of responsibilities, and minimal dependencies particularly caught my attention.

For this reason, in the Book Manager Web API project that I developed using Clean Architecture on .NET 9, I implemented Milan Jovanović’s CQRS + Decorator Pattern approach. On the Minimal API side, I used the FastEndpoints library.

While applying the Decorator Pattern, the base class used as a reference example in this document is CreateBookCommandHandler.
When an ICommandHandler<CreateBookCommand, CreateBookCommandResponse> is requested in the related endpoint, the DI container returns the CreateBookCommandHandler wrapped (decorated) first with ValidationCommandHandler and then with LoggingCommandHandler.

The Handle method inside LoggingCommandHandler performs the required logging and then calls the Handle method of the inner ICommandHandler<CreateBookCommand, CreateBookCommandResponse> instance that comes from ValidationCommandHandler.
ValidationCommandHandler follows the same approach: after applying validation rules, it calls the innermost CreateBookCommandHandler, thus completing the pipeline.

The Decorator Pattern provides a more flexible and modular architecture by using composition instead of inheritance when adding new behaviors such as validation and logging to handler classes.
By leveraging composition, it also adheres to SOLID principles such as Single Responsibility (SRP) and Open/Closed Principle (OCP).

You can find all project details and example code below 👇

---

## 🚀 Features

* ✅ Clear layer separation with Clean Architecture
* ✅ CQRS (Command Query Responsibility Segregation)
* ✅ Logging and Validation via Decorator Pattern
* ✅ Dependency Injection and service decoration with Scrutor
* ✅ No MediatR — direct routing without unnecessary abstractions
* ✅ FastEndpoints-based API routing
* ✅ Command/query validation with FluentValidation
* ✅ Success and failure handling with FluentResults
* ✅ Centralized error handling with Global Error Handling and consistent HTTP responses
* ✅ JWT-based Authentication and Authorization with Microsoft Identity
* ✅ SQL Server integration with Entity Framework Core
* ✅ Data access abstraction using Repository Pattern and Unit of Work Pattern
* ✅ Object mapping with Mapster

---

## 🧱 Tech Stack

* .NET 9
* FastEndpoints
* FluentValidation
* FluentResults
* Mapster
* Scrutor
* Entity Framework Core
* Microsoft.AspNetCore.Authentication.JwtBearer
* Microsoft.AspNetCore.Identity.EntityFrameworkCore
* Scalar.AspNetCore
  
---

## 🗂 Project Structure

```
BookManager/
├── BookManager.API/             # Minimal API layer
├── BookManager.Application/     # CQRS handlers, decorators, interfaces
├── BookManager.Domain/          # Domain models and business rules
├── BookManager.Infrastructure/  # Repositories, data access
```

---

## 🧩 CQRS: Commands & Queries

### Command Example

```csharp
public sealed record CreateBookCommand : BookCommandBase, ICommand<CreateBookCommandResponse>;

public sealed class CreateBookCommandValidator : BaseBookCommandValidator<CreateBookCommand> { }

internal sealed class CreateBookCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateBookCommand, CreateBookCommandResponse>
{
    public async Task<Result<CreateBookCommandResponse>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var exists = await unitOfWork.BookRepository.AnyAsync(b => b.ISBN == command.ISBN, cancellationToken);
        if (exists)
            return Result.Fail("A book with the same ISBN already exists."); // business logic

        var bookEntity = command.Adapt<Book>(); // Object mapping with Mapster
        unitOfWork.BookRepository.Add(bookEntity); // Entity tracking & insertion via EF Core
        await unitOfWork.SaveChangesAsync(cancellationToken); // Persist changes to database

        var response = bookEntity.Adapt<CreateBookCommandResponse>();
        return Result.Ok(response);
    }
}

public sealed class CreateBookCommandResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ISBN { get; set; } = default!;
}
```

### Query Example

```csharp
public sealed record GetBookByIdQuery(Guid Id) : IQuery<GetBookByIdQueryResponse>;

public sealed class GetBookByIdQueryValidator {...}

internal sealed class GetBookByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetBookByIdQuery, GetBookByIdQueryResponse>
{
    public async Task<Result<GetBookByIdQueryResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await unitOfWork.BookRepository.GetByIdAsync(query.Id, cancellationToken);

        if (book is null)
        {
            return Result.Fail("Book not found");
        }

        return book.Adapt<GetBookByIdQueryResponse>();
    }
}

public sealed class GetBookByIdQueryResponse {...}
```

---

## 🎀 Decorators: Validation & Logging

Validation Decorator

```csharp
internal sealed class ValidationCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var failures = await ValidationHelper.Validate(command, validators, cancellationToken); // Validation with FluentValidation

        if (!failures.Any())
            return await innerHandler.Handle(command, cancellationToken); // Call actual CommandHandler if validation passes

        return ValidationHelper.HandleValidationResult<TResponse>(failures);
    }
}
```

Logging Decorator

```csharp
internal sealed class LoggingCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    ILogger<LoggingCommandHandler<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command {CommandType}", typeof(TCommand).Name);
        var result = await innerHandler.Handle(command, cancellationToken); // Calls ValidationCommandHandler
        logger.LogInformation("Handled command {CommandType} with result: {IsSuccess}", typeof(TCommand).Name, result.IsSuccess);

        return result;
    }
}
```

Applying the Decorator Pattern with Scrutor

```csharp
// When ICommandHandler<,> is requested, concrete implementations such as CreateBookCommandHandler are resolved.

services.Scan(scan => scan.FromAssemblyOf<CreateBookCommandHandler>()
        .AddClasses(classes => classes.AssignableTo(typeof(Messaging.ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()

/*
With services.Decorate, the last registered decorator becomes the outermost
and is the first one executed.
Both the handler and decorator classes must implement
ICommandHandler<TCommand, TResponse>.
*/

services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));
services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>));
```

Pipeline order:

> [Logging 📝] → [Validation 🛡️] → [CommandHandler 📦]

Each responsibility is defined in its own layer, making the system easy to test and extend.

---

## 🧪 Testing

* Interfaces such as IApplicationDbContext and IUnitOfWork enable easy mocking
* Handler classes are small and contain only their own business logic

---

## 🧵 Minimal API Example

The CreateBookEndpoint below is a clean and effective Minimal API example built using FastEndpoints.
It integrates seamlessly with the CQRS architecture and processes commands via the ICommandHandler interface.

FastEndpoints preserves the simplicity of Minimal APIs while providing built-in support for layered architecture, validation, role-based authorization, and error handling — resulting in a more modular, testable, and maintainable project.

```csharp
namespace BookManager.API.Endpoints.Books.Commands;

public class CreateBookEndpoint(Messaging.ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler)
    : Endpoint<CreateBookCommand, CreateBookCommandResponse>
{
    public override void Configure()
    {
        Post("/books");       // HTTP POST endpoint
        Roles("Admin");       // Only users with 'Admin' role can access
    }

    public override async Task HandleAsync(CreateBookCommand req, CancellationToken ct)
    {
        var result = await handler.Handle(req, ct);

        if (result.IsFailed)
        {
            foreach (var error in result.Errors) // Collect FluentResults errors
                AddError(error.Message);

            ThrowIfAnyErrors(409); // Throws 409 Conflict if any errors were added
        }

        await SendCreatedAtAsync<GetBookByIdEndpoint>(
            new { id = result.Value.Id }, // ID of the created resource
            result.Value,
            cancellation: ct              // 201 Created + Location header
        );
    }
}
```

---

🛡 Authentication & Authorization

JWT tokens are generated via the IJwtTokenService.

With FastEndpoints, access to endpoints can be restricted by roles.
In the example below, only users with the Admin role are allowed to access the /books endpoint:

```csharp
public class CreateBookEndpoint(ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler)
    : Endpoint<CreateBookCommand, CreateBookCommandResponse>
{
    public override void Configure()
    {
        Post("/books");
        Roles("Admin"); // Only 'Admin' role is allowed
    }

    public override async Task HandleAsync(CreateBookCommand req, CancellationToken ct)
    {
       ...
    }
}
```

---

## 📦 Getting Started

```bash
# Clone the repository
$ git clone https://github.com/kaganemre/book-manager-webapi.git

# Navigate to the project folder
$ cd BookManager

# Restore dependencies
$ dotnet restore

# Run the application
$ dotnet run --project src/BookManager.API
```

---

## 📄 License

This project is licensed under the **MIT License**.  
See the [LICENSE](LICENSE) file for details.
