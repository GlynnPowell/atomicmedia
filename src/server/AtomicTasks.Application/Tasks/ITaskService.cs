namespace AtomicTasks.Application.Tasks;

public interface ITaskService
{
    System.Threading.Tasks.Task<IReadOnlyList<TaskDto>> GetTasksAsync(
        bool? isCompleted,
        string? search,
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<TaskDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<TaskDto?> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}


