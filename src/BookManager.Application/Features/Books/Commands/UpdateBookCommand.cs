using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;
using Mapster;

namespace BookManager.Application.Features.Books.Commands;

public sealed record UpdateBookCommand(Guid Id) : BookCommandBase, ICommand;
public sealed class UpdateBookCommandValidator : BaseBookCommandValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .NotEqual(Guid.Empty)
            .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}
internal sealed class UpdateBookCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateBookCommand>
{
    public async Task<Result> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var book = await unitOfWork.BookRepository.GetByIdAsync(command.Id, cancellationToken);

        if (book is null)
        {
            return Result.Fail("Kitap bulunamadı");
        }

        command.Adapt(book);
        unitOfWork.BookRepository.Update(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}