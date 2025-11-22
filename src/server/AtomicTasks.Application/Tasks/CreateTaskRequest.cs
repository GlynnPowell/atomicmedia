namespace AtomicTasks.Application.Tasks;

public sealed class CreateTaskRequest
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTime? DueDate { get; init; }
}
