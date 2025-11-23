using AtomicTasks.Application.Tasks;
using AtomicTasks.Infrastructure;
using AtomicTasks.Infrastructure.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainTask = AtomicTasks.Domain.Tasks.Task;

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

// Ensure the SQLite database and schema exist and seed initial tasks for easier testing
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AtomicTasksDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Tasks.Any())
    {
        var now = DateTime.UtcNow;

        var seedTasks = new List<DomainTask>
        {
            new()
            {
                Title = "Write technical assessment",
                Description = "Complete the Atomic Tasks full-stack assessment.",
                IsCompleted = false,
                DueDate = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Review requirements PDF",
                Description = "Re-read the assessment PDF and confirm all must-haves are covered.",
                IsCompleted = true,
                DueDate = now.AddDays(-1),
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-1),
                CreatedBy = "candidate",
                AssignedTo = "reviewer"
            },
            new()
            {
                Title = "Refactor TaskService",
                Description = "Tidy up service logic and validation before submission.",
                IsCompleted = false,
                DueDate = now.AddDays(3),
                CreatedAt = now.AddDays(-2),
                UpdatedAt = now.AddDays(-2),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Polish React UI",
                Description = "Check responsive layout, spacing, and colours.",
                IsCompleted = false,
                DueDate = now.AddDays(2),
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Add filtering and sorting tests",
                Description = "Extend tests to cover list filters and ordering.",
                IsCompleted = false,
                DueDate = now.AddDays(5),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Write README notes",
                Description = "Ensure setup and run instructions are clear for the examiner.",
                IsCompleted = true,
                DueDate = now.AddDays(-2),
                CreatedAt = now.AddDays(-4),
                UpdatedAt = now.AddDays(-2),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Implement pagination",
                Description = "Make sure long task lists are easy to navigate.",
                IsCompleted = true,
                DueDate = now.AddDays(-3),
                CreatedAt = now.AddDays(-5),
                UpdatedAt = now.AddDays(-3),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Add Playwright E2E tests",
                Description = "Cover create and toggle completion flows end-to-end.",
                IsCompleted = false,
                DueDate = now.AddDays(4),
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Hook up Vitest in CI (future)",
                Description = "Plan how unit tests would run in a pipeline.",
                IsCompleted = false,
                DueDate = null,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Review domain model alignment",
                Description = "Double-check Id, Title, IsCompleted, and DueDate match the spec.",
                IsCompleted = true,
                DueDate = now.AddDays(-1),
                CreatedAt = now.AddDays(-6),
                UpdatedAt = now.AddDays(-1),
                CreatedBy = "candidate",
                AssignedTo = "reviewer"
            },
            new()
            {
                Title = "Test validation errors",
                Description = "Confirm title max length and required field behaviour.",
                IsCompleted = false,
                DueDate = now.AddDays(2),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Add sample completed task",
                Description = "Demonstrate completed task styling and toggle behaviour.",
                IsCompleted = true,
                DueDate = now.AddDays(-4),
                CreatedAt = now.AddDays(-7),
                UpdatedAt = now.AddDays(-4),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Investigate optional features",
                Description = "Capture a few stretch ideas in METHOD.txt.",
                IsCompleted = false,
                DueDate = now.AddDays(7),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Try different filters",
                Description = "Ensure combinations of status and date filters work.",
                IsCompleted = false,
                DueDate = now.AddDays(6),
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Check API error handling",
                Description = "Verify 404s and validation problems are returned correctly.",
                IsCompleted = true,
                DueDate = now.AddDays(-2),
                CreatedAt = now.AddDays(-8),
                UpdatedAt = now.AddDays(-2),
                CreatedBy = "candidate",
                AssignedTo = "reviewer"
            },
            new()
            {
                Title = "Manual smoke test",
                Description = "Run through the UI flows as if you were the examiner.",
                IsCompleted = false,
                DueDate = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Clean up dead code",
                Description = "Remove any unused enums or DTOs left from refactors.",
                IsCompleted = true,
                DueDate = now.AddDays(-5),
                CreatedAt = now.AddDays(-9),
                UpdatedAt = now.AddDays(-5),
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Run backend tests",
                Description = "Execute dotnet test on the solution.",
                IsCompleted = false,
                DueDate = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Run frontend unit tests",
                Description = "Execute npm test in the client folder.",
                IsCompleted = false,
                DueDate = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Run Playwright tests",
                Description = "Execute npm run test:e2e with API and UI running.",
                IsCompleted = false,
                DueDate = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            },
            new()
            {
                Title = "Final submission check",
                Description = "Quick pass over code, README, and METHOD.txt.",
                IsCompleted = false,
                DueDate = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "candidate",
                AssignedTo = "candidate"
            }
        };

        dbContext.Tasks.AddRange(seedTasks);
        dbContext.SaveChanges();
    }
}

var tasksGroup = app.MapGroup("/api/tasks");

// Helper endpoint to expose distinct CreatedBy/AssignedTo values for dropdown filters
tasksGroup.MapGet("/filter-values", async (
        AtomicTasksDbContext db,
        CancellationToken cancellationToken) =>
    {
        var createdBy = await db.Tasks
            .Where(t => t.CreatedBy != null && t.CreatedBy != string.Empty)
            .Select(t => t.CreatedBy!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var assignedTo = await db.Tasks
            .Where(t => t.AssignedTo != null && t.AssignedTo != string.Empty)
            .Select(t => t.AssignedTo!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        return Results.Ok(new { createdBy, assignedTo });
    });

tasksGroup.MapGet("/", async (
        ITaskService service,
        bool? isCompleted,
        DateTime? dueFrom,
        DateTime? dueTo,
        string? createdBy,
        string? assignedTo,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
    {
        var tasks = await service.GetTasksAsync(
            isCompleted,
            dueFrom,
            dueTo,
            createdBy,
            assignedTo,
            search,
            sortBy,
            sortDirection,
            page,
            pageSize,
            cancellationToken);
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
            ["title"] = new[] { "Title must be at most 100 characters." }
        };
    }

    return null;
}
