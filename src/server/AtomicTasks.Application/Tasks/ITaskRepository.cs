namespace AtomicTasks.Application.Tasks;

using DomainTask = AtomicTasks.Domain.Tasks.Task;

public interface ITaskRepository
{
    System.Threading.Tasks.Task<IReadOnlyList<DomainTask>> GetTasksAsync(
        bool? isCompleted,
        DateTime? dueFrom,
        DateTime? dueTo,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<DomainTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<DomainTask> AddAsync(DomainTask task, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task UpdateAsync(DomainTask task, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task DeleteAsync(DomainTask task, CancellationToken cancellationToken = default);
}


