namespace BookManager.Domain.Entities;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public List<Book> Books { get; set; } = new();
}