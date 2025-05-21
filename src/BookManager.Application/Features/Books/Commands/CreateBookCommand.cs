using BookManager.Application.Common.Exceptions;
using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using BookManager.Domain.Entities;
using Mapster;

namespace BookManager.Application.Features.Books.Commands;

public sealed record CreateBookCommandRequest : BookRequestBase;
public sealed class CreateBookCommandRequestValidator : BaseBookRequestValidator<CreateBookCommandRequest> { }
public sealed class CreateBookCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<CreateBookCommandResponse> HandleAsync(CreateBookCommandRequest req, CancellationToken ct)
    {
        var exists = await _unitOfWork.BookRepository.AnyAsync(b => b.ISBN == req.ISBN, ct);
        if (exists)
            throw new ConflictException("Aynı ISBN ile zaten bir kitap var.");

        var bookEntity = req.Adapt<Book>();
        _unitOfWork.BookRepository.Add(bookEntity);
        await _unitOfWork.SaveChangesAsync(ct);

        return bookEntity.Adapt<CreateBookCommandResponse>();
    }
}
public sealed class CreateBookCommandResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ISBN { get; set; } = default!;
}