using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using BookManager.Domain.Entities;
using FluentResults;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Application.Features.Books.Commands;

public sealed record CreateBookCommand : BookCommandBase, ICommand<CreateBookCommandResponse>;
public sealed class CreateBookCommandValidator : BaseBookCommandValidator<CreateBookCommand> { }
internal sealed class CreateBookCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateBookCommand, CreateBookCommandResponse>
{
    public async Task<Result<CreateBookCommandResponse>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var exists = await db.Books.AnyAsync(b => b.ISBN == command.ISBN, cancellationToken);
        if (exists)
            return Result.Fail("A book with the same ISBN already exists.");

        var bookEntity = command.Adapt<Book>();
        db.Books.Add(bookEntity);
        await db.SaveChangesAsync(cancellationToken);
        
        var response = new CreateBookCommandResponse(bookEntity.Id, bookEntity.Title, bookEntity.ISBN);
        return Result.Ok(response);
    }
}
public sealed record CreateBookCommandResponse(Guid Id, string Title, string ISBN);