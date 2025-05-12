namespace BookManager.Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IBookRepository BookRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}