using BookManager.Application.Features.Books.Queries;
using BookManager.Application.Interfaces;
using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetAllBooksEndpoint : EndpointWithoutRequest<IReadOnlyList<GetAllBooksResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllBooksEndpoint(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public override void Configure()
    {
        Get("/api/books");
        AllowAnonymous();
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var books = await _unitOfWork.BookRepository.GetAll().ToListAsync(ct);
        var booksDto = books.Adapt<IReadOnlyList<GetAllBooksResponse>>();

        await SendAsync(booksDto, cancellation: ct);
    }
}