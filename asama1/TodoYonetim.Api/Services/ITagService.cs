using TodoYonetim.Api.DTOs;

namespace TodoYonetim.Api.Services;

public interface ITagService
{
    Task<IReadOnlyList<TagResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TagResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TagResponse> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
