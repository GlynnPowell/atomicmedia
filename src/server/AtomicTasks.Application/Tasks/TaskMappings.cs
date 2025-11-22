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
            Status = entity.Status,
            Priority = entity.Priority,
            DueDate = entity.DueDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
}


