using System.ComponentModel.DataAnnotations;

namespace TodoLog.Api.DTOs;

public sealed class CreateTaskLogRequest
{
    [Range(1, int.MaxValue)]
    public int TodoId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

public sealed record TaskLogResponse(int Id, int TodoId, string Title, DateTime CreatedAt, DateTime LoggedAt);
