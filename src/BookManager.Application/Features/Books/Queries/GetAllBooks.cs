namespace BookManager.Application.Features.Books.Queries;
public sealed record GetAllBooksRequest();
public sealed class GetAllBooksResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string ISBN { get; set; } = default!;
    public int? PageCount { get; set; }
    public string Source { get; set; } = default!;
}