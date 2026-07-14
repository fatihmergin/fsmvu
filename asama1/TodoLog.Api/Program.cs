using Microsoft.EntityFrameworkCore;
using TodoLog.Api.Data;
using TodoLog.Api.Repositories;
using TodoLog.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<LogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITaskLogRepository, TaskLogRepository>();
builder.Services.AddScoped<ITaskLogService, TaskLogService>();

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
