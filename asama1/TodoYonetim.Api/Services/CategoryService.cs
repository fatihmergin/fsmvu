using TodoYonetim.Api.DTOs;
using TodoYonetim.Api.Exceptions;
using TodoYonetim.Api.Models;
using TodoYonetim.Api.Repositories;

namespace TodoYonetim.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => new CategoryResponse(x.Id, x.Name)).ToList();
    }

    public async Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ResourceNotFoundException("Kategori bulunamadı.");
        return new CategoryResponse(item.Id, item.Name);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new AppValidationException("Kategori adı boş bırakılamaz.");
        }

        if (await _repository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new ConflictException("Bu kategori zaten mevcut.");
        }

        var category = new Category { Name = name };
        await _repository.AddAsync(category, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return new CategoryResponse(category.Id, category.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ResourceNotFoundException("Kategori bulunamadı.");

        if (await _repository.IsInUseAsync(id, cancellationToken))
        {
            throw new ConflictException("Görevlerde kullanılan kategori silinemez.");
        }

        _repository.Delete(category);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
