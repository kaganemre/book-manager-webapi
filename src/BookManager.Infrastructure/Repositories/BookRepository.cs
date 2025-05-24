using BookManager.Application.Interfaces;
using BookManager.Domain.Entities;
using BookManager.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Infrastructure.Repositories;

public class BookRepository(ApplicationDbContext context) : Repository<Book>(context), IBookRepository
{
    public override async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}
