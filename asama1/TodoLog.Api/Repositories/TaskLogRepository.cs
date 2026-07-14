using Microsoft.EntityFrameworkCore;
using TodoLog.Api.Data;
using TodoLog.Api.Models;

namespace TodoLog.Api.Repositories;

public class TaskLogRepository : ITaskLogRepository
{
    private readonly LogDbContext _dbContext;

    public TaskLogRepository(LogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TaskLogEntry>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TaskLogs.AsNoTracking().OrderByDescending(x => x.LoggedAt).ToListAsync(cancellationToken);
    }

    public Task<TaskLogEntry?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.TaskLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task AddAsync(TaskLogEntry item, CancellationToken cancellationToken)
    {
        return _dbContext.TaskLogs.AddAsync(item, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
