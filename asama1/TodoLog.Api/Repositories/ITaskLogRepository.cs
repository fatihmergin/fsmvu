using TodoLog.Api.Models;

namespace TodoLog.Api.Repositories;

public interface ITaskLogRepository
{
    Task<IReadOnlyList<TaskLogEntry>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskLogEntry?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(TaskLogEntry item, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
