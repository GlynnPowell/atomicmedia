namespace AtomicTasks.Application.Tasks;

using DomainTask = AtomicTasks.Domain.Tasks.Task;

public static class TaskMappings
{
    public static TaskDto ToDto(this DomainTask entity) =>
        new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted,
            DueDate = entity.DueDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CreatedBy = entity.CreatedBy,
            AssignedTo = entity.AssignedTo
        };
}


