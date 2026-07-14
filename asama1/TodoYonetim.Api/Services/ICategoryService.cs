using TodoYonetim.Api.DTOs;

namespace TodoYonetim.Api.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
