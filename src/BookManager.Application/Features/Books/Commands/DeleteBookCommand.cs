using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;

namespace BookManager.Application.Features.Books.Commands;

public sealed record DeleteBookCommand(Guid Id) : ICommand;
public sealed class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.Id)
             .NotEmpty()
             .Must(id => Guid.TryParse(id.ToString(), out _))
             .NotEqual(Guid.Empty)
             .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}
internal sealed class DeleteBookCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var book = await unitOfWork.BookRepository.GetByIdAsync(command.Id, cancellationToken);

        if (book is null)
        {
            return Result.Fail("Kitap bulunamadı");
        }

        unitOfWork.BookRepository.Remove(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}