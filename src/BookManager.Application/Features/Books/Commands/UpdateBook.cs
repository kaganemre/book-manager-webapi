using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using FluentResults;
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

    public async Task<Result<bool>> HandleAsync(UpdateBookCommandRequest req, CancellationToken ct)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(req.Id, ct);

        if (book is null)
            return Result.Fail("Kitap bulunamadı");

        req.Adapt(book);
        _unitOfWork.BookRepository.Update(book);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Ok(true);
    }


}