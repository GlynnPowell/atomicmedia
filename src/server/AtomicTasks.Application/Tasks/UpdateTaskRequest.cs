namespace AtomicTasks.Application.Tasks;

public sealed class UpdateTaskRequest
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool IsCompleted { get; init; }

    public DateTime? DueDate { get; init; }
}
