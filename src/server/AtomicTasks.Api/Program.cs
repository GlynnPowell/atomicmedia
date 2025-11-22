using AtomicTasks.Application.Tasks;
using AtomicTasks.Infrastructure;
using AtomicTasks.Infrastructure.Tasks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:4173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=AtomicTasks.db";

builder.Services.AddDbContext<AtomicTasksDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

// Ensure the SQLite database and schema exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AtomicTasksDbContext>();
    dbContext.Database.EnsureCreated();
}

var tasksGroup = app.MapGroup("/api/tasks");

tasksGroup.MapGet("/", async (
        ITaskService service,
        bool? isCompleted,
        string? search,
        CancellationToken cancellationToken) =>
    {
        var tasks = await service.GetTasksAsync(isCompleted, search, cancellationToken);
        return Results.Ok(tasks);
    });

tasksGroup.MapGet("/{id:int}", async (
        int id,
        ITaskService service,
        CancellationToken cancellationToken) =>
    {
        var task = await service.GetByIdAsync(id, cancellationToken);
        return task is null
            ? Results.NotFound()
            : Results.Ok(task);
    });

tasksGroup.MapPost("/", async (
        CreateTaskRequest request,
        ITaskService service,
        CancellationToken cancellationToken) =>
    {
        var validationErrors = ValidateTitle(request.Title);
        if (validationErrors is not null)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var created = await service.CreateAsync(request, cancellationToken);
        return Results.Created($"/api/tasks/{created.Id}", created);
    });

tasksGroup.MapPut("/{id:int}", async (
        int id,
        UpdateTaskRequest request,
        ITaskService service,
        CancellationToken cancellationToken) =>
    {
        var validationErrors = ValidateTitle(request.Title);
        if (validationErrors is not null)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var updated = await service.UpdateAsync(id, request, cancellationToken);
        return updated is null
            ? Results.NotFound()
            : Results.Ok(updated);
    });

tasksGroup.MapDelete("/{id:int}", async (
        int id,
        ITaskService service,
        CancellationToken cancellationToken) =>
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    });

app.Run();

static Dictionary<string, string[]>? ValidateTitle(string? title)
{
    if (string.IsNullOrWhiteSpace(title))
    {
        return new Dictionary<string, string[]>
        {
            ["title"] = new[] { "Title is required." }
        };
    }

    if (title.Length > 100)
    {
        return new Dictionary<string, string[]>
        {
            ["title"] = new[] { "Title must be at most 200 characters." }
        };
    }

    return null;
}
