namespace AtomicTasks.Application.Tasks;

using AtomicTasks.Domain.Tasks;

public sealed class CreateTaskRequest
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public TaskPriority? Priority { get; init; }

    public DateTime? DueDate { get; init; }
}


