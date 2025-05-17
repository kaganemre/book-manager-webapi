using BookManager.Application.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Application.Features.Books.Queries;

// public sealed record GetAllBooksRequest();
public sealed class GetAllBooksQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllBooksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<GetAllBooksResponse>> HandleAsync(CancellationToken ct)
    {
        var books = await _unitOfWork.BookRepository.GetAll()
                            .AsNoTracking()
                            .ToListAsync(ct);

        return books.Adapt<IReadOnlyList<GetAllBooksResponse>>();
    }
}
public sealed class GetAllBooksResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string ISBN { get; set; } = default!;
    public int? PageCount { get; set; }
    public string Source { get; set; } = default!;
}