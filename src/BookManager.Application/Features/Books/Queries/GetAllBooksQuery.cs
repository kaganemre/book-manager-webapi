using BookManager.Application.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Application.Features.Books.Queries;

public sealed class GetAllBooksQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllBooksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IReadOnlyList<GetAllBooksQueryResponse>> HandleAsync(CancellationToken ct)
    {
        var books = await _unitOfWork.BookRepository.GetAll().ToListAsync(ct);
        return books.Adapt<IReadOnlyList<GetAllBooksQueryResponse>>();
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