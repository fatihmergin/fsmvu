using TodoYonetim.Api.DTOs;
using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Repositories;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(TodoQueryRequest query, CancellationToken cancellationToken);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(TodoItem item, CancellationToken cancellationToken);
    void Delete(TodoItem item);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
