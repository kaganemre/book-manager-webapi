using BookManager.Application.Interfaces;
using FastEndpoints;
using FluentResults;
using FluentValidation;

namespace BookManager.Application.Features.Books.Commands;

public sealed record DeleteBookCommandRequest(Guid Id);
public sealed class DeleteBookCommandRequestValidator : Validator<DeleteBookCommandRequest>
{
    public DeleteBookCommandRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}

public sealed class DeleteBookCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> HandleAsync(DeleteBookCommandRequest req, CancellationToken ct)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(req.Id, ct);

        if (book is null)
            return Result.Fail("Kitap bulunamadı");

        _unitOfWork.BookRepository.Remove(book);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}