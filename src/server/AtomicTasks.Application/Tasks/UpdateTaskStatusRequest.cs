namespace AtomicTasks.Application.Tasks;

using DomainTaskStatus = AtomicTasks.Domain.Tasks.TaskStatus;

public sealed class UpdateTaskStatusRequest
{
    public DomainTaskStatus Status { get; init; }
}


