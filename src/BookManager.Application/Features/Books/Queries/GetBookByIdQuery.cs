using BookManager.Application.Interfaces;
using FastEndpoints;
using FluentValidation;
using Mapster;

namespace BookManager.Application.Features.Books.Queries;

public sealed record GetBookByIdQueryRequest(Guid Id);
public sealed class GetBookByIdQueryValidator : Validator<GetBookByIdQueryRequest>
{
    public GetBookByIdQueryValidator()
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
    public async Task<GetBookByIdQueryResponse> HandleAsync(GetBookByIdQueryRequest req, CancellationToken ct)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(req.Id, ct);
        return book.Adapt<GetBookByIdQueryResponse>();
    }
}
public sealed class GetBookByIdQueryResponse
{
    public Guid Id { get; set; }
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