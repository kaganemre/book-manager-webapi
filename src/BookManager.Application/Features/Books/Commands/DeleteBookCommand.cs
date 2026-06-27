using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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
             .WithMessage("A valid book ID must be specified.");
    }
}
internal sealed class DeleteBookCommandHandler(IApplicationDbContext db)
    : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (book is null)
        {
            return Result.Fail("Book not found.");
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}