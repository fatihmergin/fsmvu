using TodoLog.Api.DTOs;

namespace TodoLog.Api.Services;

public interface ITaskLogService
{
    Task<IReadOnlyList<TaskLogResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskLogResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TaskLogResponse> CreateAsync(CreateTaskLogRequest request, CancellationToken cancellationToken);
}
