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
        ITaskRepository repository,
        bool? isCompleted,
        string? search,
        CancellationToken cancellationToken) =>
    {
        var tasks = await repository.GetTasksAsync(isCompleted, search, cancellationToken);
        return Results.Ok(tasks.Select(t => t.ToDto()));
    });

tasksGroup.MapGet("/{id:int}", async (
        int id,
        ITaskRepository repository,
        CancellationToken cancellationToken) =>
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        return task is null
            ? Results.NotFound()
            : Results.Ok(task.ToDto());
    });

tasksGroup.MapPost("/", async (
        CreateTaskRequest request,
        ITaskRepository repository,
        CancellationToken cancellationToken) =>
    {
        var validationErrors = ValidateTitle(request.Title);
        if (validationErrors is not null)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var entity = new AtomicTasks.Domain.Tasks.Task
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            DueDate = request.DueDate,
            IsCompleted = false
        };

        var created = await repository.AddAsync(entity, cancellationToken);
        var dto = created.ToDto();

        return Results.Created($"/api/tasks/{dto.Id}", dto);
    });

tasksGroup.MapPut("/{id:int}", async (
        int id,
        UpdateTaskRequest request,
        ITaskRepository repository,
        CancellationToken cancellationToken) =>
    {
        var validationErrors = ValidateTitle(request.Title);
        if (validationErrors is not null)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return Results.NotFound();
        }

        existing.Title = request.Title.Trim();
        existing.Description = request.Description;
        existing.IsCompleted = request.IsCompleted;
        existing.DueDate = request.DueDate;

        await repository.UpdateAsync(existing, cancellationToken);

        return Results.Ok(existing.ToDto());
    });

tasksGroup.MapDelete("/{id:int}", async (
        int id,
        ITaskRepository repository,
        CancellationToken cancellationToken) =>
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return Results.NotFound();
        }

        await repository.DeleteAsync(existing, cancellationToken);

        return Results.NoContent();
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
