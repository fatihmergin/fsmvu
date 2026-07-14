using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Repositories;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken cancellationToken);
    Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Tag tag, CancellationToken cancellationToken);
    void Delete(Tag tag);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
