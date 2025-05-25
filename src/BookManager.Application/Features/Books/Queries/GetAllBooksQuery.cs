using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Application.Features.Books.Queries;

public sealed record GetAllBooksQuery() : IQuery<IReadOnlyList<GetAllBooksQueryResponse>>;
internal sealed class GetAllBooksQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllBooksQuery, IReadOnlyList<GetAllBooksQueryResponse>>
{
    public async Task<Result<IReadOnlyList<GetAllBooksQueryResponse>>> Handle(GetAllBooksQuery query, CancellationToken cancellationToken)
    {
        var books = await unitOfWork.BookRepository
            .GetAll()
            .ToListAsync(cancellationToken);

        var booksDto = books.Adapt<IReadOnlyList<GetAllBooksQueryResponse>>();

        return Result.Ok(booksDto);
    }
}
public sealed class GetAllBooksQueryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string ISBN { get; set; } = default!;
    public int? PageCount { get; set; }
    public string Source { get; set; } = default!;
}