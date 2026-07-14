using Microsoft.EntityFrameworkCore;
using TodoYonetim.Api.Data;
using TodoYonetim.Api.DTOs;
using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _dbContext;

    public TodoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(TodoQueryRequest query, CancellationToken cancellationToken)
    {
        IQueryable<TodoItem> items = _dbContext.Todos
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.TodoTags)
            .ThenInclude(x => x.Tag);

        items = query.Status.ToLowerInvariant() switch
        {
            "completed" => items.Where(x => x.IsCompleted),
            "pending" => items.Where(x => !x.IsCompleted),
            _ => items
        };

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            items = items.Where(x =>
                x.Title.Contains(search) ||
                (x.Description != null && x.Description.Contains(search)) ||
                (x.Category != null && x.Category.Name.Contains(search)) ||
                x.TodoTags.Any(t => t.Tag.Name.Contains(search)));
        }

        if (query.CategoryId.HasValue)
        {
            items = items.Where(x => x.CategoryId == query.CategoryId.Value);
        }

        if (query.TagId.HasValue)
        {
            items = items.Where(x => x.TodoTags.Any(t => t.TagId == query.TagId.Value));
        }

        var ascending = query.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);
        items = ApplySorting(items, query.SortBy, ascending);

        return await items.ToListAsync(cancellationToken);
    }

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Todos
            .Include(x => x.Category)
            .Include(x => x.TodoTags)
            .ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        return _dbContext.Todos.AddAsync(item, cancellationToken).AsTask();
    }

    public void Delete(TodoItem item)
    {
        _dbContext.Todos.Remove(item);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<TodoItem> ApplySorting(IQueryable<TodoItem> items, string sortBy, bool ascending)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "title" => ascending
                ? items.OrderBy(x => x.Title).ThenBy(x => x.Id)
                : items.OrderByDescending(x => x.Title).ThenByDescending(x => x.Id),
            "duedate" => ascending
                ? items.OrderBy(x => x.DueDate == null).ThenBy(x => x.DueDate).ThenBy(x => x.Id)
                : items.OrderBy(x => x.DueDate == null).ThenByDescending(x => x.DueDate).ThenByDescending(x => x.Id),
            "priority" => ascending
                ? items.OrderBy(x => x.Priority).ThenBy(x => x.Id)
                : items.OrderByDescending(x => x.Priority).ThenByDescending(x => x.Id),
            "category" => ascending
                ? items.OrderBy(x => x.Category == null).ThenBy(x => x.Category!.Name).ThenBy(x => x.Id)
                : items.OrderBy(x => x.Category == null).ThenByDescending(x => x.Category!.Name).ThenByDescending(x => x.Id),
            "status" => ascending
                ? items.OrderBy(x => x.IsCompleted).ThenByDescending(x => x.CreatedAt)
                : items.OrderByDescending(x => x.IsCompleted).ThenByDescending(x => x.CreatedAt),
            _ => ascending
                ? items.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id)
                : items.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
        };
    }
}
