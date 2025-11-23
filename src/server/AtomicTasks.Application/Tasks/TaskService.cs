using DomainTask = AtomicTasks.Domain.Tasks.Task;

namespace AtomicTasks.Application.Tasks;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<TaskDto>> GetTasksAsync(
        bool? isCompleted,
        DateTime? dueFrom,
        DateTime? dueTo,
        string? createdBy,
        string? assignedTo,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _repository.GetTasksAsync(
            isCompleted,
            dueFrom,
            dueTo,
            createdBy,
            assignedTo,
            search,
            sortBy,
            sortDirection,
            page,
            pageSize,
            cancellationToken);
        return tasks.Select(t => t.ToDto()).ToArray();
    }

    public async System.Threading.Tasks.Task<TaskDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        return task?.ToDto();
    }

    public async System.Threading.Tasks.Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new DomainTask
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            DueDate = request.DueDate,
            IsCompleted = false,
            // For this assessment we don't model real users, but we still
            // allow the optional CreatedBy/AssignedTo fields to be set to
            // demonstrate the extended data model from the bonus section.
            CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy)
                ? "demo-user"
                : request.CreatedBy.Trim(),
            AssignedTo = string.IsNullOrWhiteSpace(request.AssignedTo)
                ? "demo-user"
                : request.AssignedTo.Trim()
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return created.ToDto();
    }

    public async System.Threading.Tasks.Task<TaskDto?> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Title = request.Title.Trim();
        existing.Description = request.Description;
        existing.IsCompleted = request.IsCompleted;
        existing.DueDate = request.DueDate;
        existing.CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy)
            ? null
            : request.CreatedBy.Trim();
        existing.AssignedTo = string.IsNullOrWhiteSpace(request.AssignedTo)
            ? null
            : request.AssignedTo.Trim();

        await _repository.UpdateAsync(existing, cancellationToken);

        return existing.ToDto();
    }

    public async System.Threading.Tasks.Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        await _repository.DeleteAsync(existing, cancellationToken);
        return true;
    }
}


