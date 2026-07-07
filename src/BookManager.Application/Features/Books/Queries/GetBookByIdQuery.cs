using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Application.Features.Books.Queries;

public sealed record GetBookByIdQuery(Guid Id) : IQuery<GetBookByIdQueryResponse>;

internal sealed class GetBookByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetBookByIdQuery, GetBookByIdQueryResponse>
{
    public async Task<Result<GetBookByIdQueryResponse>> Handle(GetBookByIdQuery query,
        CancellationToken cancellationToken)
    {
        var book = await db.Books
            .AsNoTracking()
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == query.Id, cancellationToken);

        if (book is null)
        {
            return Result.Fail("Book not found.");
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