using FastEndpoints;
using FluentValidation;

namespace BookManager.Application.Features.Books.Commands;
public sealed record DeleteBookRequest(Guid Id);
public sealed class DeleteBookRequestValidator : Validator<DeleteBookRequest>
{
    public DeleteBookRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}