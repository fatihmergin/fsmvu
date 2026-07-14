using System.Net.Http.Json;
using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Clients;

public class TaskLogClient : ITaskLogClient
{
    private readonly HttpClient _httpClient;

    public TaskLogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LogTaskCreatedAsync(TodoItem item, CancellationToken cancellationToken)
    {
        var request = new TaskCreatedLogRequest(item.Id, item.Title, item.CreatedAt);

        try
        {
            await _httpClient.PostAsJsonAsync("api/logs", request, cancellationToken);
        }
        catch (HttpRequestException)
        {
        }
        catch (TaskCanceledException)
        {
        }
    }

    private sealed record TaskCreatedLogRequest(int TodoId, string Title, DateTime CreatedAt);
}
