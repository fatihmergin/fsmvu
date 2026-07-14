namespace TodoYonetim.Api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<TodoItem> Todos { get; set; } = new List<TodoItem>();
}
