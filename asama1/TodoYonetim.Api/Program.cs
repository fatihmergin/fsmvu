using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TodoYonetim.Api.Clients;
using TodoYonetim.Api.Data;
using TodoYonetim.Api.Middleware;
using TodoYonetim.Api.Repositories;
using TodoYonetim.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddHttpClient<ITaskLogClient, TaskLogClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:LogApi"] ?? "http://localhost:5051");
    client.Timeout = TimeSpan.FromSeconds(3);
});

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
