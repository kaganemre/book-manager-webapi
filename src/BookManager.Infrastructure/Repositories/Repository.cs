using System.Linq.Expressions;
using BookManager.Application.Interfaces;
using BookManager.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    public IQueryable<TEntity> GetAll() => _dbSet.AsNoTracking();
    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
    }
    public void Add(TEntity entity) => _dbSet.Add(entity);
    public void Update(TEntity entity) => _dbSet.Update(entity);
    public void Remove(TEntity entity) => _dbSet.Remove(entity);
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }
}