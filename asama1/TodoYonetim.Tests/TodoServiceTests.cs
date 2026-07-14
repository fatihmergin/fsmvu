using Xunit;
using TodoYonetim.Api.Clients;
using TodoYonetim.Api.DTOs;
using TodoYonetim.Api.Exceptions;
using TodoYonetim.Api.Models;
using TodoYonetim.Api.Repositories;
using TodoYonetim.Api.Services;

namespace TodoYonetim.Tests;

public class TodoServiceTests
{
    [Fact]
    public async Task CreateAsync_GecmisTarihteHataVerir()
    {
        var setup = CreateSetup();
        var request = new CreateTodoRequest
        {
            Title = "Test görevi",
            DueDate = DateTime.Today.AddDays(-1),
            Priority = TodoPriority.Orta
        };

        await Assert.ThrowsAsync<AppValidationException>(() => setup.Service.CreateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_GoreviKaydederVeLogServisiniCagirir()
    {
        var setup = CreateSetup();
        setup.Categories.Items.Add(new Category { Id = 4, Name = "Yazılım" });
        setup.Tags.Items.Add(new Tag { Id = 7, Name = "Acil" });
        var request = new CreateTodoRequest
        {
            Title = " API geliştir ",
            Description = " CRUD işlemleri ",
            DueDate = DateTime.Today.AddDays(2),
            Priority = TodoPriority.Yuksek,
            CategoryId = 4,
            TagIds = new List<int> { 7 }
        };

        var result = await setup.Service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("API geliştir", result.Title);
        Assert.Single(setup.Todos.Items);
        Assert.Equal(1, setup.Logger.CallCount);
        Assert.Equal("Yazılım", result.Category?.Name);
        Assert.Single(result.Tags);
    }

    [Fact]
    public async Task ToggleAsync_TamamlanmaDurumunuDegistirir()
    {
        var setup = CreateSetup();
        setup.Todos.Items.Add(new TodoItem { Id = 3, Title = "Görev", IsCompleted = false });

        var result = await setup.Service.ToggleAsync(3, CancellationToken.None);

        Assert.True(result.IsCompleted);
        Assert.NotNull(result.CompletedAt);
    }

    [Fact]
    public async Task DeleteAsync_GoreviSiler()
    {
        var setup = CreateSetup();
        setup.Todos.Items.Add(new TodoItem { Id = 5, Title = "Silinecek görev" });

        await setup.Service.DeleteAsync(5, CancellationToken.None);

        Assert.Empty(setup.Todos.Items);
    }

    private static TestSetup CreateSetup()
    {
        var todos = new FakeTodoRepository();
        var categories = new FakeCategoryRepository();
        var tags = new FakeTagRepository();
        var logger = new FakeTaskLogClient();
        var service = new TodoService(todos, categories, tags, logger);
        return new TestSetup(service, todos, categories, tags, logger);
    }

    private sealed record TestSetup(
        TodoService Service,
        FakeTodoRepository Todos,
        FakeCategoryRepository Categories,
        FakeTagRepository Tags,
        FakeTaskLogClient Logger);

    private sealed class FakeTodoRepository : ITodoRepository
    {
        public List<TodoItem> Items { get; } = new();

        public Task<IReadOnlyList<TodoItem>> GetAllAsync(TodoQueryRequest query, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<TodoItem>>(Items);
        }

        public Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(TodoItem item, CancellationToken cancellationToken)
        {
            item.Id = Items.Count == 0 ? 1 : Items.Max(x => x.Id) + 1;
            Items.Add(item);
            return Task.CompletedTask;
        }

        public void Delete(TodoItem item)
        {
            Items.Remove(item);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        public List<Category> Items { get; } = new();

        public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Category>>(Items);
        }

        public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(Category category, CancellationToken cancellationToken)
        {
            Items.Add(category);
            return Task.CompletedTask;
        }

        public void Delete(Category category)
        {
            Items.Remove(category);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTagRepository : ITagRepository
    {
        public List<Tag> Items { get; } = new();

        public Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Tag>>(Items);
        }

        public Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
        }

        public Task<IReadOnlyList<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            var idSet = ids.ToHashSet();
            return Task.FromResult<IReadOnlyList<Tag>>(Items.Where(x => idSet.Contains(x.Id)).ToList());
        }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(Tag tag, CancellationToken cancellationToken)
        {
            Items.Add(tag);
            return Task.CompletedTask;
        }

        public void Delete(Tag tag)
        {
            Items.Remove(tag);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTaskLogClient : ITaskLogClient
    {
        public int CallCount { get; private set; }

        public Task LogTaskCreatedAsync(TodoItem item, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.CompletedTask;
        }
    }
}
