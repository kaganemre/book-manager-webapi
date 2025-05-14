using BookManager.Application.Interfaces;
using BookManager.Infrastructure.Context;
using BookManager.Infrastructure.Repositories;

namespace BookManager.Infrastructure.UoW
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IBookRepository? _bookRepository;
        private ICategoryRepository? _categoryRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IBookRepository BookRepository =>
            _bookRepository ??= new BookRepository(_context);
        public ICategoryRepository CategoryRepository =>
            _categoryRepository ??= new CategoryRepository(_context);
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}