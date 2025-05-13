using BookManager.Application.Features.Books.Shared;

namespace BookManager.Application.Features.Books.Commands;
public sealed record CreateBookRequest : BookRequestBase;
public sealed class CreateBookRequestValidator : BaseBookRequestValidator<CreateBookRequest> { }
public sealed class CreateBookResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ISBN { get; set; } = default!;
}