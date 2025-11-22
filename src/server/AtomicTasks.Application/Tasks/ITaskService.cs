namespace AtomicTasks.Application.Tasks;

public interface ITaskService
{
    System.Threading.Tasks.Task<IReadOnlyList<TaskDto>> GetTasksAsync(
        bool? isCompleted,
        DateTime? dueFrom,
        DateTime? dueTo,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<TaskDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<TaskDto?> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}


