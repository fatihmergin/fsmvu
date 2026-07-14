using System.ComponentModel.DataAnnotations;

namespace TodoYonetim.Api.DTOs;

public sealed class CreateTagRequest
{
    [Required(ErrorMessage = "Etiket adı zorunludur.")]
    [MaxLength(50, ErrorMessage = "Etiket adı en fazla 50 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;
}

public sealed record TagResponse(int Id, string Name);
