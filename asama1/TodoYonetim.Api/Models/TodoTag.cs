namespace TodoYonetim.Api.Models;

public class TodoTag
{
    public int TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; } = null!;
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
