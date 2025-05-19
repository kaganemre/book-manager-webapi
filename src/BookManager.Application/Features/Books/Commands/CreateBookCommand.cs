using BookManager.Application.Features.Books.Shared;
using BookManager.Application.Interfaces;
using BookManager.Domain.Entities;
using FluentResults;
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

    public async Task<Result<CreateBookResponse>> HandleAsync(CreateBookCommandRequest req, CancellationToken ct)
    {
        var exists = await _unitOfWork.BookRepository.AnyAsync(b => b.ISBN == req.ISBN);
        if (exists)
            return Result.Fail("Bu ISBN ile zaten bir kitap var.");

        var bookEntity = req.Adapt<Book>();
        _unitOfWork.BookRepository.Add(bookEntity);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = bookEntity.Adapt<CreateBookResponse>();

        return Result.Ok(response);

    }
}
public sealed class CreateBookResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ISBN { get; set; } = default!;
}