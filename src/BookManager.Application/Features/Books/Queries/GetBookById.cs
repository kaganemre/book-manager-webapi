using FastEndpoints;
using FluentValidation;

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