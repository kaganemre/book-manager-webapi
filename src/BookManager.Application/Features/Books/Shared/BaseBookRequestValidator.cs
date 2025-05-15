using FastEndpoints;
using FluentValidation;

namespace BookManager.Application.Features.Books.Shared;

public abstract class BaseBookRequestValidator<T> : Validator<T>
    where T : BookRequestBase
{
    protected BaseBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Başlık alanı zorunludur.")
            .MaximumLength(150)
            .WithMessage("Başlık en fazla 150 karakter olabilir.");

        RuleFor(x => x.Author)
            .NotEmpty()
            .WithMessage("Yazar adı zorunludur.")
            .MaximumLength(100)
            .WithMessage("Yazar adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Açıklama en fazla 1000 karakter olabilir.");

        RuleFor(x => x.ISBN)
            .NotEmpty()
            .WithMessage("ISBN alanı zorunludur.")
            .Matches(@"^\d{10}(\d{3})?$")
            .WithMessage("ISBN 10 veya 13 haneli olmalıdır.");

        RuleFor(x => x.PageCount)
            .GreaterThan(0)
            .When(x => x.PageCount.HasValue)
            .WithMessage("Sayfa sayısı sıfırdan büyük olmalıdır.");

        RuleFor(x => x.PublishedDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Yayın tarihi bugünden ileri bir tarih olamaz.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stok miktarı sıfır veya daha büyük olmalıdır.");

        RuleFor(x => x.Source)
            .NotEmpty()
            .WithMessage("Kaynak alanı zorunludur.")
            .MaximumLength(255)
            .WithMessage("Kaynak en fazla 255 karakter olabilir.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Kategori kimliği pozitif bir değer olmalıdır.");
    }
}