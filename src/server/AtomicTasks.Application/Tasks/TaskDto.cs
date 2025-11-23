namespace AtomicTasks.Application.Tasks;

public sealed class TaskDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool IsCompleted { get; init; }

    public DateTime? DueDate { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public string? CreatedBy { get; init; }

    public string? AssignedTo { get; init; }
}


