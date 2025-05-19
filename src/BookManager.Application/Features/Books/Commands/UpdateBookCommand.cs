using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using FluentValidation;
using Mapster;

namespace BookManager.Application.Features.Books.Commands;

public sealed record UpdateBookCommandRequest(
    Guid Id
) : BookRequestBase;
public sealed class UpdateBookCommandRequestValidator : BaseBookRequestValidator<UpdateBookCommandRequest>
{
    public UpdateBookCommandRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}
public sealed class UpdateBookCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task HandleAsync(UpdateBookCommandRequest req, CancellationToken ct)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(req.Id, ct)
                    ?? throw new KeyNotFoundException("Kitap bulunamadı");

        req.Adapt(book);
        _unitOfWork.BookRepository.Update(book);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}