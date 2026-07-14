using TodoYonetim.Api.DTOs;
using TodoYonetim.Api.Exceptions;
using TodoYonetim.Api.Models;
using TodoYonetim.Api.Repositories;

namespace TodoYonetim.Api.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TagResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => new TagResponse(x.Id, x.Name)).ToList();
    }

    public async Task<TagResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ResourceNotFoundException("Etiket bulunamadı.");
        return new TagResponse(item.Id, item.Name);
    }

    public async Task<TagResponse> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new AppValidationException("Etiket adı boş bırakılamaz.");
        }

        if (await _repository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new ConflictException("Bu etiket zaten mevcut.");
        }

        var tag = new Tag { Name = name };
        await _repository.AddAsync(tag, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return new TagResponse(tag.Id, tag.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var tag = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ResourceNotFoundException("Etiket bulunamadı.");

        if (await _repository.IsInUseAsync(id, cancellationToken))
        {
            throw new ConflictException("Görevlerde kullanılan etiket silinemez.");
        }

        _repository.Delete(tag);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
