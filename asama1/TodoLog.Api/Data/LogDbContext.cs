using Microsoft.EntityFrameworkCore;
using TodoLog.Api.Models;

namespace TodoLog.Api.Data;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {
    }

    public DbSet<TaskLogEntry> TaskLogs => Set<TaskLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskLogEntry>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(120);
            entity.HasIndex(x => x.TodoId);
            entity.HasIndex(x => x.LoggedAt);
        });
    }
}
