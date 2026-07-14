namespace TodoYonetim.Api.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
}
