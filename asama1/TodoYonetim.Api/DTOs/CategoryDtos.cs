using System.ComponentModel.DataAnnotations;

namespace TodoYonetim.Api.DTOs;

public sealed class CreateCategoryRequest
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [MaxLength(80, ErrorMessage = "Kategori adı en fazla 80 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;
}

public sealed record CategoryResponse(int Id, string Name);
