using BookManager.Domain.Common;

namespace BookManager.Domain.Entities;

public sealed class Book : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string? Description { get; set; }
    public string ISBN { get; set; } = default!;
    public int? PageCount { get; set; }
    public DateOnly PublishedDate { get; set; } = default!;
    public int StockQuantity { get; set; }
    public string Source { get; set; } = default!;

    public Category Category { get; set; } = default!;
}