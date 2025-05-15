namespace BookManager.Application.Features.Books.Shared;

public abstract record BookRequestBase
{
    public string Title { get; init; } = default!;
    public string Author { get; init; } = default!;
    public string? Description { get; init; }
    public string ISBN { get; init; } = default!;
    public int? PageCount { get; init; }
    public DateOnly PublishedDate { get; init; }
    public int StockQuantity { get; init; }
    public string Source { get; init; } = default!;
    public int CategoryId { get; init; }
}