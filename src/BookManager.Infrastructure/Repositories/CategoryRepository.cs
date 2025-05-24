using BookManager.Application.Interfaces;
using BookManager.Domain.Entities;
using BookManager.Infrastructure.Context;

namespace BookManager.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext context)
    : Repository<Category>(context), ICategoryRepository
{
}