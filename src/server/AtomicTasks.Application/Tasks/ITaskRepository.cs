namespace AtomicTasks.Application.Tasks;

using DomainTask = AtomicTasks.Domain.Tasks.Task;
using DomainTaskStatus = AtomicTasks.Domain.Tasks.TaskStatus;

public interface ITaskRepository
{
    System.Threading.Tasks.Task<IReadOnlyList<DomainTask>> GetTasksAsync(
        DomainTaskStatus? status,
        string? search,
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<DomainTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<DomainTask> AddAsync(DomainTask task, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task UpdateAsync(DomainTask task, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task DeleteAsync(DomainTask task, CancellationToken cancellationToken = default);
}


