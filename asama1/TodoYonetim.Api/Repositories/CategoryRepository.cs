using Microsoft.EntityFrameworkCore;
using TodoYonetim.Api.Data;
using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Categories.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        var normalized = name.ToLower();
        return _dbContext.Categories.AnyAsync(x => x.Name.ToLower() == normalized, cancellationToken);
    }

    public Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Todos.AnyAsync(x => x.CategoryId == id, cancellationToken);
    }

    public Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        return _dbContext.Categories.AddAsync(category, cancellationToken).AsTask();
    }

    public void Delete(Category category)
    {
        _dbContext.Categories.Remove(category);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
