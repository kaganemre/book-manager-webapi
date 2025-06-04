# 📚 Book Manager

CQRS Pattern ile neredeyse özdeşleşmiş olan MediatR'ın ticari sürüme geçme kararından sonra ücretsiz alternatifler arasından Decorator Pattern ile CQRS Pattern yaklaşımı (Milan Jovanović) öne çıkıyor. Esnek mimarisi, net sorumluluk ayrımları ve bağımlılıklarının az olması dikkatimi çekti. Bu sebeple .NET 9 ile Clean Architecture temelli geliştirdiğim Book Manager Web API projesinde Milan Jovanović'in yaklaşımını uyguladım. Minimal API tarafında ise FastEndpoints kütüphanesini kullandım.

Decorator Pattern'ı uygularken bu dokümanda örnek olarak aldığımız temel sınıf CreateBookCommandHandler'dır. İlgili endpoint'te ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler'ı istediğimizde DI Container bize CreateBookCommandHandler sınıfını önce ValidationCommandHandler sonra da LoggingCommandHandler decorator sınıflarıyla sararak(dekore ederek) gönderir.

LoggingCommandHandler sınıfındaki Handle metodu gerekli loglama işlemlerini yaptıktan sonra sınıfın constructor'ına ValidationCommandHandler'dan gelen ICommandHandler<CreateBookCommand, CreateBookCommandResponse> innerHandler objesinin Handle metodunu çağırır. ValidationCommandHandler da aynı yolu izleyip gerekli validasyon kurallarını kontrol ettikten sonra en içteki CreateBookCommandHandler'ın Handle metodunu çağırarak pipeline'ı sonlandırır.

Decorator Pattern yaklaşımının, handler sınıflarına validasyon ve loglama gibi yeni davranışlar kazandırırken inheritance yerine composition'ı kullanması daha esnek ve modüler bir mimari sağlıyor. Ayrıca composition ile genişleyerek net sorumluluk ayrımı (SRP) ve değişime kapalı genişlemeye açık (OCP) gibi SOLID prensiplerine de uyuyor.

Projeye ait tüm detayları ve örnek kodları aşağıda bulabilirsiniz 👇

---

## 🚀 Features

* ✅ Clean Architecture ile katmanlar arası net ayrım
* ✅ CQRS (Command Query Responsibility Segregation)
* ✅ Decorator Pattern ile Logging ve Validation
* ✅ Scrutor ile Dependency Injection ve servis dekorasyonu
* ✅ MediatR kullanılmadan — doğrudan yönlendirme yapılır ve gereksiz soyutlamalar olmadan çalışır
* ✅ FastEndpoints tabanlı API yönlendirmesi
* ✅ FluentValidation ile command/query doğrulama
* ✅ FluentResults ile başarılı ve başarısız durumları yönetme
* ✅ Global Error Handling ile merkezi hata yönetimi ve tutarlı HTTP yanıtları
* ✅ JWT tabanlı Authentication ve Microsoft Identity ile Authorization
* ✅ Entity Framework Core ile SQL Server entegrasyonu
* ✅ Repository Pattern ve Unit of Work Pattern ile veri erişim soyutlaması
* ✅ Mapster ile object mapping


---

## 🧱 Tech Stack

* .NET 9
* C# 13
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

## 🗂 Proje Yapısı

```
BookManager/
├── BookManager.API/             # Minimal API layer
├── BookManager.Application/     # CQRS handlers, decorators, interfaces
├── BookManager.Domain/          # Domain models and business rules
├── BookManager.Infrastructure/  # Repositories, data access
```

---

## 🧩 CQRS: Commands & Queries

### Command Örneği

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
            return Result.Fail("Aynı ISBN ile zaten bir kitap var."); // business logic(iş mantığı)...

        var bookEntity = command.Adapt<Book>();	//Mapster ile object mapping
        unitOfWork.BookRepository.Add(bookEntity); // EF Core ile entity ekleme & takibi
        await unitOfWork.SaveChangesAsync(cancellationToken); // Beritabanına kayıt etme.

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

### Query Örneği

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
            return Result.Fail("Kitap bulunamadı");
        }

        return book.Adapt<GetBookByIdQueryResponse>();
    }
}

public sealed class GetBookByIdQueryResponse {...}
```

---

## 🎀 Decorators: Validation & Logging

Validation Decorator:

```csharp
internal sealed class ValidationCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var failures = await ValidationHelper.Validate(command, validators, cancellationToken); // FluentValidation ile validasyon kontrolü

        if (!failures.Any())
            return await innerHandler.Handle(command, cancellationToken); // Doğrulama başarılıysa ilgili CommandHandler'ın Handle metodunu çağır

        return ValidationHelper.HandleValidationResult<TResponse>(failures);
    }
}
```

Logging Decorator:

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
        var result = await innerHandler.Handle(command, cancellationToken); // ValidationCommandHandler'ın Handle metodunu çağır
        logger.LogInformation("Handled command {CommandType} with result: {IsSuccess}", typeof(TCommand).Name, result.IsSuccess);

        return result;
    }
}
```

Scrutor ile Decorator Pattern'ı uygulama:

```csharp
// ICommandHandler<,> çağırıldığında CreateBookCommandHandler gibi concrete sınıfları gönder.

services.Scan(scan => scan.FromAssemblyOf<CreateBookCommandHandler>()
		.AddClasses(classes => classes.AssignableTo(typeof(Messaging.ICommandHandler<,>)), publicOnly: false)
			.AsImplementedInterfaces() 
			.WithScopedLifetime()
			
/*services.Decorate ile en son tanımlanan decorator en dışa sarılır ve ilk çalışan decorator olur. Dekore etme işlemi için Handle ve Decorator sınıfların ICommandHandler<in TCommand, TResponse> interface'ini implement etmesi gerekir.*/

services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>)); 
services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>));
```

Katmanlar:

> [Validation 🛡️] → [Logging 📝] → [CommandHandler 📦]

Her bir sorumluluk ayrı katmanlarda tanımlanır, kolayca test edilebilir ve gerektiğinde genişletilebilir yapıdadır.

---

## 🧪 Testing

* IApplicationDbContext ve IUnitOfWork gibi arayüzler, birimlerin kolayca mock’lanmasını sağlar
* Handler sınıfları küçük ve sadece kendi iş mantığını içerir

---

## 🧵 Minimal API Örneği

Aşağıda yer alan CreateBookEndpoint, FastEndpoints kullanılarak oluşturulmuş sade ve etkili bir Minimal API örneğidir. Bu yapı, CQRS mimarisiyle uyumlu çalışır ve komutları ICommandHandler arayüzü üzerinden işler. FastEndpoints; Minimal API'nin sadeliğini korurken, katmanlı yapı, validation, role tabanlı yetkilendirme ve hata yönetimi gibi özellikleri yerleşik olarak sunar. Bu sayede proje daha modüler, test edilebilir ve bakımı kolay hale gelir.

```csharp
namespace BookManager.API.Endpoints.Books.Commands;

public class CreateBookEndpoint(Messaging.ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler)
    : Endpoint<CreateBookCommand, CreateBookCommandResponse>
{
    public override void Configure()
    {
        Post("/books");       // HTTP POST metodu tanımı
        Roles("Admin");       // Yalnızca 'Admin' rolü erişebilir
    }

    public override async Task HandleAsync(CreateBookCommand req, CancellationToken ct)
    {
        var result = await handler.Handle(req, ct);

        if (result.IsFailed)
        {
            foreach (var error in result.Errors) // FluentResults hataları toplanır
                AddError(error.Message); 

            ThrowIfAnyErrors(409); // AddError() ile toplanan hata/hatalar varsa 409 Conflict Exception fırlatılır.
        }

        await SendCreatedAtAsync<GetBookByIdEndpoint>(
            new { id = result.Value.Id }, // Oluşturulan kaydın ID’siyle
            result.Value,
            cancellation: ct			// 201 Created + Location header
        );
    }
}
```

---

🛡 Kimlik Doğrulama ve Yetkilendirme

JWT token’ları IJwtTokenService aracılığıyla oluşturulur.

FastEndpoints ile, endpoint'lere erişim rollere göre sınırlanabilir. Aşağıdaki örnekte yalnızca "Admin" rolüne sahip kullanıcıların /books endpoint’ine erişmesine izin verilmektedir:

```csharp
public class CreateBookEndpoint(ICommandHandler<CreateBookCommand, CreateBookCommandResponse> handler)
    : Endpoint<CreateBookCommand, CreateBookCommandResponse>
{
    public override void Configure()
    {
        Post("/books");
        Roles("Admin"); // Yalnızca 'Admin' rolü erişebilir
    }

    public override async Task HandleAsync(CreateBookCommand req, CancellationToken ct)
    {
       ...
    }
}
```

---

## 📦 Başlarken

```bash
# Clone repo
$ git clone https://github.com/kaganemre/book-manager-webapi.git

# Proje klasörüne geçin
$ cd BookManager

# Bağımlılıkları geri yükleyin
$ dotnet restore

# Uygulamayı çalıştırın
$ dotnet run --project src/BookManager.API
```

---

## ✅ Yapılacaklar / Geliştirmeler

*

---

## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır.
