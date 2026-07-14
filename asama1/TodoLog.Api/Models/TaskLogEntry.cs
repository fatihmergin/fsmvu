namespace TodoLog.Api.Models;

public class TaskLogEntry
{
    public int Id { get; set; }
    public int TodoId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}
