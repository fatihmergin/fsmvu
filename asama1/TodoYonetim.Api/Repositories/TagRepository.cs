using Microsoft.EntityFrameworkCore;
using TodoYonetim.Api.Data;
using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _dbContext;

    public TagRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Tags.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var distinctIds = ids.Distinct().ToArray();
        return await _dbContext.Tags.Where(x => distinctIds.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        var normalized = name.ToLower();
        return _dbContext.Tags.AnyAsync(x => x.Name.ToLower() == normalized, cancellationToken);
    }

    public Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.TodoTags.AnyAsync(x => x.TagId == id, cancellationToken);
    }

    public Task AddAsync(Tag tag, CancellationToken cancellationToken)
    {
        return _dbContext.Tags.AddAsync(tag, cancellationToken).AsTask();
    }

    public void Delete(Tag tag)
    {
        _dbContext.Tags.Remove(tag);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
