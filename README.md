# 📚 Book Manager

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-success)
![CQRS](https://img.shields.io/badge/CQRS-Decorator%20Pattern-blueviolet)
![EF Core](https://img.shields.io/badge/EF%20Core-blue?logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Auth-black?logo=jsonwebtokens&logoColor=white)
![FluentValidation](https://img.shields.io/badge/FluentValidation-green)
![Scalar](https://img.shields.io/badge/API%20Docs-Scalar-6C47FF)
![License](https://img.shields.io/github/license/kaganemre/book-manager-webapi)

After MediatR — which has almost become synonymous with the CQRS pattern — announced its transition to a commercial licence, the CQRS approach implemented with the Decorator Pattern (popularised by Milan Jovanović) stood out as a compelling free alternative. Its flexible architecture, clear separation of responsibilities, and minimal dependencies particularly caught my attention.

For this reason, in the Book Manager Web API project I developed using Clean Architecture on .NET 9, I implemented Milan Jovanović's CQRS + Decorator Pattern approach alongside ASP.NET Core Minimal API.

While applying the Decorator Pattern, the base class used as a reference example in this document is `CreateBookCommandHandler`. When an `ICommandHandler<CreateBookCommand, CreateBookCommandResponse>` is requested at the related endpoint, the DI container returns the `CreateBookCommandHandler` wrapped (decorated) first with `ValidationCommandHandler` and then with `LoggingCommandHandler`.

The `Handle` method inside `LoggingCommandHandler` performs the required logging and then calls the `Handle` method of the inner `ICommandHandler<CreateBookCommand, CreateBookCommandResponse>` instance that comes from `ValidationCommandHandler`. `ValidationCommandHandler` follows the same approach: after applying validation rules, it calls the innermost `CreateBookCommandHandler`, thus completing the pipeline.

The Decorator Pattern provides a more flexible and modular architecture by using composition instead of inheritance when adding new behaviours such as validation and logging to handler classes. By leveraging composition, it also adheres to SOLID principles such as Single Responsibility (SRP) and Open/Closed Principle (OCP).

---

## 🚀 Features

* ✅ Clear layer separation with Clean Architecture
* ✅ CQRS (Command Query Responsibility Segregation)
* ✅ Logging and Validation via Decorator Pattern
* ✅ Dependency Injection and service decoration with Scrutor
* ✅ No MediatR — direct handler routing without unnecessary abstractions
* ✅ ASP.NET Core Minimal API with static extension class endpoints
* ✅ Direct EF Core data access via `IApplicationDbContext` — no Repository or Unit of Work overhead
* ✅ Command/query validation with FluentValidation
* ✅ Success and failure handling with FluentResults
* ✅ Centralised error handling with Global Exception Handler and consistent HTTP responses
* ✅ JWT-based Authentication and Authorisation with ASP.NET Core Identity
* ✅ SQL Server integration with Entity Framework Core
* ✅ Object mapping with Mapster
* ✅ Interactive API documentation with Scalar

---

## 🧱 Tech Stack

* .NET 9
* ASP.NET Core Minimal API
* FluentValidation
* FluentResults
* Mapster
* Scrutor
* Entity Framework Core
* JWT Bearer Authentication
* ASP.NET Core Identity
* Scalar.AspNetCore

---

## 🗂 Project Structure

```
BookManager/
├── BookManager.Domain/          # Domain entities and base types
├── BookManager.Application/     # CQRS handlers, decorators, validators, interfaces
├── BookManager.Infrastructure/  # EF Core DbContext, Identity, JWT, migrations, seed data
├── BookManager.API/             # Minimal API layer — endpoints, middleware, extensions
```

---

## 🧩 CQRS: Commands & Queries

### Command Example

```csharp
public sealed record CreateBookCommand : BookCommandBase, ICommand<CreateBookCommandResponse>;

public sealed class CreateBookCommandValidator : BaseBookCommandValidator<CreateBookCommand> { }

internal sealed class CreateBookCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateBookCommand, CreateBookCommandResponse>
{
    public async Task<Result<CreateBookCommandResponse>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var exists = await db.Books.AnyAsync(b => b.ISBN == command.ISBN, cancellationToken);
        if (exists)
            return Result.Fail("A book with the same ISBN already exists.");

        var bookEntity = command.Adapt<Book>();
        db.Books.Add(bookEntity);
        await db.SaveChangesAsync(cancellationToken);

        var response = new CreateBookCommandResponse(bookEntity.Id, bookEntity.Title, bookEntity.ISBN);
        return Result.Ok(response);
    }
}

public sealed record CreateBookCommandResponse(Guid Id, string Title, string ISBN);
```

### Query Example

```csharp
public sealed record GetBookByIdQuery(Guid Id) : IQuery<GetBookByIdQueryResponse>;

public sealed class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery> { ... }

internal sealed class GetBookByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetBookByIdQuery, GetBookByIdQueryResponse>
{
    public async Task<Result<GetBookByIdQueryResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await db.Books
            .AsNoTracking()
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == query.Id, cancellationToken);

        if (book is null)
            return Result.Fail("Book not found.");

        return book.Adapt<GetBookByIdQueryResponse>();
    }
}
```

---

## 🎀 Decorators: Validation & Logging

### Validation Decorator

```csharp
internal sealed class ValidationCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var failures = await ValidationHelper.Validate(command, validators, cancellationToken);

        if (!failures.Any())
            return await innerHandler.Handle(command, cancellationToken);

        return ValidationHelper.HandleValidationResult<TResponse>(failures);
    }
}
```

### Logging Decorator

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
        var result = await innerHandler.Handle(command, cancellationToken);
        logger.LogInformation("Handled command {CommandType} with result: {IsSuccess}", typeof(TCommand).Name, result.IsSuccess);

        return result;
    }
}
```

### Applying the Decorator Pattern with Scrutor

```csharp
services.Scan(scan => scan.FromAssemblyOf<CreateBookCommandHandler>()
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

// With services.Decorate, the last registered decorator becomes the outermost
// and is the first one executed.
services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));
services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>));
```

Pipeline order:

> [Logging 📝] → [Validation 🛡️] → [CommandHandler 📦]

Each responsibility is defined in its own layer, making the system easy to test and extend.

---

## 🌐 Minimal API Endpoints

Endpoints are implemented as static extension classes and registered via a single call in `Program.cs`.

```csharp
public static class CreateBookEndpoint
{
    public static void MapCreateBook(this IEndpointRouteBuilder app)
    {
        app.MapPost("/books", async (
            [FromBody] CreateBookCommand command,
            ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(command, ct);

            if (result.IsFailed)
                return Results.Conflict(result.Errors.Select(e => e.Message));

            return Results.CreatedAtRoute(
                GetBookByIdEndpoint.Name,
                new { id = result.Value.Id },
                result.Value);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}
```

All endpoints are grouped per feature and registered in `Program.cs` through extension methods:

```csharp
app.MapAuthEndpoints();
app.MapBookEndpoints();
```

---

## 🛡 Authentication & Authorization

JWT tokens are generated via `IJwtTokenService` and validated using JWT Bearer Authentication. Role-based authorization is applied directly on each endpoint:

```csharp
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.RequireAuthorization(policy => policy.RequireRole("Admin", "User"))
```

ASP.NET Core Identity manages user registration, password hashing, and role assignment. Roles (`Admin`, `User`) are seeded automatically on startup.

---

## 🧪 Testing

* `IApplicationDbContext` can be easily mocked for unit testing handlers
* Handler classes are small and contain only their own business logic
* The Decorator Pattern separates cross-cutting concerns, keeping handlers focused and testable

---

## 📦 Getting Started

```bash
# Clone the repository
git clone https://github.com/kaganemre/book-manager-webapi.git

# Navigate to the project folder
cd BookManager

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project src/BookManager.API
```

> Migrations are applied automatically on startup. Seed data (roles and a default admin user) is also inserted automatically.

---

## 📄 Licence

This project is licensed under the **MIT Licence**.
See the [LICENSE](LICENSE) file for details.