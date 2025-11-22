namespace AtomicTasks.Application.Tasks;

using AtomicTasks.Domain.Tasks;

public sealed class TaskDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public TaskStatus Status { get; init; }

    public TaskPriority Priority { get; init; }

    public DateTime? DueDate { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}


