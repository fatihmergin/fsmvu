using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Clients;

public interface ITaskLogClient
{
    Task LogTaskCreatedAsync(TodoItem item, CancellationToken cancellationToken);
}
