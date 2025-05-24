using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using BookManager.Domain.Entities;
using FluentResults;
using Mapster;

namespace BookManager.Application.Features.Books.Commands;

public sealed record CreateBookCommand : BookCommandBase, ICommand<CreateBookCommandResponse>;
public sealed class CreateBookCommandValidator : BaseBookCommandValidator<CreateBookCommand> { }
internal sealed class CreateBookCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateBookCommand, CreateBookCommandResponse>
{
    public async Task<Result<CreateBookCommandResponse>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var exists = await unitOfWork.BookRepository.AnyAsync(b => b.ISBN == command.ISBN, cancellationToken);
        if (exists)
            return Result.Fail("Aynı ISBN ile zaten bir kitap var.");

        var bookEntity = command.Adapt<Book>();
        unitOfWork.BookRepository.Add(bookEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = bookEntity.Adapt<CreateBookCommandResponse>();
        return Result.Ok(response);
    }
}
public sealed class CreateBookCommandResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ISBN { get; set; } = default!;
}