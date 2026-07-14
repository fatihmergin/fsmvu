using Microsoft.EntityFrameworkCore;
using TodoYonetim.Api.Models;

namespace TodoYonetim.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TodoTag> TodoTags => Set<TodoTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Description).HasMaxLength(600);
            entity.HasIndex(x => x.IsCompleted);
            entity.HasIndex(x => x.DueDate);
            entity.HasOne(x => x.Category)
                .WithMany(x => x.Todos)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(80);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<TodoTag>(entity =>
        {
            entity.HasKey(x => new { x.TodoItemId, x.TagId });
            entity.HasOne(x => x.TodoItem)
                .WithMany(x => x.TodoTags)
                .HasForeignKey(x => x.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Tag)
                .WithMany(x => x.TodoTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
