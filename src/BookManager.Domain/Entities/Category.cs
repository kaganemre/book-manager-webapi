using BookManager.Domain.Common;

namespace BookManager.Domain.Entities;
public sealed class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public List<Book> Books { get; set; } = new();
}