using System.ComponentModel.DataAnnotations;
using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.DTOs;

public sealed class CreateTodoRequest
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [MaxLength(120, ErrorMessage = "Başlık en fazla 120 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(600, ErrorMessage = "Açıklama en fazla 600 karakter olabilir.")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TodoPriority Priority { get; set; } = TodoPriority.Orta;

    public int? CategoryId { get; set; }

    public List<int> TagIds { get; set; } = new();
}

public sealed class UpdateTodoRequest
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [MaxLength(120, ErrorMessage = "Başlık en fazla 120 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(600, ErrorMessage = "Açıklama en fazla 600 karakter olabilir.")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TodoPriority Priority { get; set; }

    public int? CategoryId { get; set; }

    public List<int> TagIds { get; set; } = new();
}

public sealed class TodoQueryRequest
{
    public string Status { get; set; } = "all";
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public int? TagId { get; set; }
    public string SortBy { get; set; } = "created";
    public string SortDirection { get; set; } = "desc";
}

public sealed record TodoResponse(
    int Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    TodoPriority Priority,
    bool IsCompleted,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    CategoryResponse? Category,
    IReadOnlyList<TagResponse> Tags,
    bool IsOverdue);
