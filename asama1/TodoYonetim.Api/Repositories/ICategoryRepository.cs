using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
    void Delete(Category category);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
