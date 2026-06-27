using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

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
            .WithMessage("A valid book ID must be specified.");
    }
}
internal sealed class UpdateBookCommandHandler(IApplicationDbContext db)
    : ICommandHandler<UpdateBookCommand>
{
    public async Task<Result> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (book is null)
        {
            return Result.Fail("Book not found.");
        }

        command.Adapt(book);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}