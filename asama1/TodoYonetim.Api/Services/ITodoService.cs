using TodoYonetim.Api.DTOs;

namespace TodoYonetim.Api.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoResponse>> GetAllAsync(TodoQueryRequest query, CancellationToken cancellationToken);
    Task<TodoResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TodoResponse> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken);
    Task<TodoResponse> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken);
    Task<TodoResponse> ToggleAsync(int id, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
