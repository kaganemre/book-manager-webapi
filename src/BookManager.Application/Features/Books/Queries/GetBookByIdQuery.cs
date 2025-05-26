using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using FastEndpoints;
using FluentResults;
using FluentValidation;
using Mapster;

namespace BookManager.Application.Features.Books.Queries;

public sealed record GetBookByIdQuery(Guid Id) : IQuery<GetBookByIdQueryResponse>;
public sealed class GetBookByIdQueryValidator : Validator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .NotEqual(Guid.Empty)
            .WithMessage("Geçerli bir kitap kimliği belirtilmelidir.");
    }
}
internal sealed class GetBookByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetBookByIdQuery, GetBookByIdQueryResponse>
{
    public async Task<Result<GetBookByIdQueryResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await unitOfWork.BookRepository.GetByIdAsync(query.Id, cancellationToken);

        if (book is null)
        {
            return Result.Fail("Kitap bulunamadı");
        }

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