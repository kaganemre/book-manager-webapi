using BookManager.Application.Interfaces;
using FastEndpoints;
using FluentResults;
using FluentValidation;
using Mapster;

namespace BookManager.Application.Features.Books.Queries;

public sealed record GetBookByIdRequest(Guid Id);
public sealed class GetBookByIdRequestValidator : Validator<GetBookByIdRequest>
{
    public GetBookByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}
public sealed class GetBookByIdQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    public GetBookByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetBookByIdResponse>> HandleAsync(GetBookByIdRequest req, CancellationToken ct)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(req.Id, ct);

        if (book is null)
        {
            return Result.Fail("Kitap bulunamadı");
        }

        return book.Adapt<GetBookByIdResponse>();
    }
}
public sealed class GetBookByIdResponse
{
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string? Description { get; set; }
    public string ISBN { get; set; } = default!;
    public int? PageCount { get; set; }
    public DateOnly PublishedDate { get; set; } = default!;
    public int StockQuantity { get; set; }
    public string Source { get; set; } = default!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
}