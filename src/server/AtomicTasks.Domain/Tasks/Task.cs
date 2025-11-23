namespace AtomicTasks.Domain.Tasks;

public class Task
{
    // Core fields required by the assessment spec
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime? DueDate { get; set; }

    // Extra metadata (optional, useful for auditing)
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Optional user-related fields (bonus section of the brief)
    public string? CreatedBy { get; set; }

    public string? AssignedTo { get; set; }
}
