using BookManager.Application.Features.Books.Queries;
using BookManager.Application.Interfaces;
using FastEndpoints;
using FluentResults;
using Mapster;

namespace BookManager.API.Endpoints.Books.Queries;

public class GetBookByIdEndpoint : Endpoint<GetBookByIdRequest, Result<GetBookByIdResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetBookByIdEndpoint(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public override void Configure()
    {
        Get("/api/book/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetBookByIdRequest req, CancellationToken ct)
    {
        var book = await _unitOfWork.BookRepository.GetByIdAsync(req.Id, ct);

        if (book is null)
        {
            await SendAsync(Result.Fail("Kitap bulunamadı"), 404, ct);
            return;
        }

        var bookDto = book.Adapt<GetBookByIdResponse>();

        await SendAsync(Result.Ok(bookDto), 200, ct);
    }
}