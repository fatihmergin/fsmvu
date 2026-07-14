using TodoLog.Api.DTOs;
using TodoLog.Api.Models;
using TodoLog.Api.Repositories;

namespace TodoLog.Api.Services;

public class TaskLogService : ITaskLogService
{
    private readonly ITaskLogRepository _repository;

    public TaskLogService(ITaskLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TaskLogResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<TaskLogResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<TaskLogResponse> CreateAsync(CreateTaskLogRequest request, CancellationToken cancellationToken)
    {
        var item = new TaskLogEntry
        {
            TodoId = request.TodoId,
            Title = request.Title.Trim(),
            CreatedAt = request.CreatedAt,
            LoggedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(item, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    private static TaskLogResponse Map(TaskLogEntry item)
    {
        return new TaskLogResponse(item.Id, item.TodoId, item.Title, item.CreatedAt, item.LoggedAt);
    }
}
