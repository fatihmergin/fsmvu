using TodoYonetim.Api.Clients;
using TodoYonetim.Api.DTOs;
using TodoYonetim.Api.Exceptions;
using TodoYonetim.Api.Models;
using TodoYonetim.Api.Repositories;

namespace TodoYonetim.Api.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ITaskLogClient _taskLogClient;

    public TodoService(
        ITodoRepository todoRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        ITaskLogClient taskLogClient)
    {
        _todoRepository = todoRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _taskLogClient = taskLogClient;
    }

    public async Task<IReadOnlyList<TodoResponse>> GetAllAsync(TodoQueryRequest query, CancellationToken cancellationToken)
    {
        var items = await _todoRepository.GetAllAsync(query, cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<TodoResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var item = await FindTodoAsync(id, cancellationToken);
        return Map(item);
    }

    public async Task<TodoResponse> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        ValidateTitle(request.Title);
        ValidateNewDueDate(request.DueDate);

        var category = await ResolveCategoryAsync(request.CategoryId, cancellationToken);
        var tags = await ResolveTagsAsync(request.TagIds, cancellationToken);

        var item = new TodoItem
        {
            Title = request.Title.Trim(),
            Description = NormalizeOptionalText(request.Description),
            DueDate = request.DueDate,
            Priority = request.Priority,
            CategoryId = category?.Id,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };

        item.TodoTags = tags.Select(tag => new TodoTag { TodoItem = item, TagId = tag.Id, Tag = tag }).ToList();

        await _todoRepository.AddAsync(item, cancellationToken);
        await _todoRepository.SaveChangesAsync(cancellationToken);
        await _taskLogClient.LogTaskCreatedAsync(item, cancellationToken);

        return Map(item);
    }

    public async Task<TodoResponse> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        ValidateTitle(request.Title);

        var item = await FindTodoAsync(id, cancellationToken);
        var category = await ResolveCategoryAsync(request.CategoryId, cancellationToken);
        var tags = await ResolveTagsAsync(request.TagIds, cancellationToken);

        item.Title = request.Title.Trim();
        item.Description = NormalizeOptionalText(request.Description);
        item.DueDate = request.DueDate;
        item.Priority = request.Priority;
        item.CategoryId = category?.Id;
        item.Category = category;

        var requestedTagIds = tags.Select(x => x.Id).ToHashSet();
        var linksToRemove = item.TodoTags.Where(x => !requestedTagIds.Contains(x.TagId)).ToList();
        foreach (var link in linksToRemove)
        {
            item.TodoTags.Remove(link);
        }

        var existingTagIds = item.TodoTags.Select(x => x.TagId).ToHashSet();
        foreach (var tag in tags.Where(x => !existingTagIds.Contains(x.Id)))
        {
            item.TodoTags.Add(new TodoTag { TodoItemId = item.Id, TodoItem = item, TagId = tag.Id, Tag = tag });
        }

        await _todoRepository.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task<TodoResponse> ToggleAsync(int id, CancellationToken cancellationToken)
    {
        var item = await FindTodoAsync(id, cancellationToken);
        item.IsCompleted = !item.IsCompleted;
        item.CompletedAt = item.IsCompleted ? DateTime.UtcNow : null;
        await _todoRepository.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var item = await FindTodoAsync(id, cancellationToken);
        _todoRepository.Delete(item);
        await _todoRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<TodoItem> FindTodoAsync(int id, CancellationToken cancellationToken)
    {
        return await _todoRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ResourceNotFoundException("Görev bulunamadı.");
    }

    private async Task<Category?> ResolveCategoryAsync(int? categoryId, CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue)
        {
            return null;
        }

        return await _categoryRepository.GetByIdAsync(categoryId.Value, cancellationToken)
            ?? throw new ResourceNotFoundException("Seçilen kategori bulunamadı.");
    }

    private async Task<IReadOnlyList<Tag>> ResolveTagsAsync(IEnumerable<int> tagIds, CancellationToken cancellationToken)
    {
        var ids = tagIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return Array.Empty<Tag>();
        }

        var tags = await _tagRepository.GetByIdsAsync(ids, cancellationToken);
        if (tags.Count != ids.Length)
        {
            throw new ResourceNotFoundException("Seçilen etiketlerden biri bulunamadı.");
        }

        return tags;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new AppValidationException("Başlık boş bırakılamaz.");
        }
    }

    private static void ValidateNewDueDate(DateTime? dueDate)
    {
        if (dueDate.HasValue && dueDate.Value.Date < DateTime.Today)
        {
            throw new AppValidationException("Yeni görev için geçmiş bir son tarih seçilemez.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static TodoResponse Map(TodoItem item)
    {
        var category = item.Category is null ? null : new CategoryResponse(item.Category.Id, item.Category.Name);
        var tags = item.TodoTags
            .Select(x => new TagResponse(x.Tag.Id, x.Tag.Name))
            .OrderBy(x => x.Name)
            .ToList();
        var isOverdue = item.DueDate.HasValue && item.DueDate.Value.Date < DateTime.Today && !item.IsCompleted;

        return new TodoResponse(
            item.Id,
            item.Title,
            item.Description,
            item.DueDate,
            item.Priority,
            item.IsCompleted,
            item.CreatedAt,
            item.CompletedAt,
            category,
            tags,
            isOverdue);
    }
}
