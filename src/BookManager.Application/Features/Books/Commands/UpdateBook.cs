using BookManager.Application.Features.Books.Shared;
using FluentValidation;

namespace BookManager.Application.Features.Books.Commands;
public sealed record UpdateBookRequest(
    Guid Id
) : BookRequestBase;

public sealed class UpdateBookRequestValidator : BaseBookRequestValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}