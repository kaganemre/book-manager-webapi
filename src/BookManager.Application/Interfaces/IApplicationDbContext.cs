using BookManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Application.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Book> Books { get; }
    DbSet<Category> Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}