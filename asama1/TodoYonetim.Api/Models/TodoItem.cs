namespace TodoYonetim.Api.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Orta;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
}
