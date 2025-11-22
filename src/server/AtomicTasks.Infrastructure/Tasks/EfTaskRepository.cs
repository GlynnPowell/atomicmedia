using Microsoft.EntityFrameworkCore;

namespace AtomicTasks.Infrastructure.Tasks;

using ApplicationTaskRepository = AtomicTasks.Application.Tasks.ITaskRepository;
using DomainTask = AtomicTasks.Domain.Tasks.Task;
using DomainTaskStatus = AtomicTasks.Domain.Tasks.TaskStatus;

public sealed class EfTaskRepository : ApplicationTaskRepository
{
    private readonly AtomicTasksDbContext _dbContext;

    public EfTaskRepository(AtomicTasksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<DomainTask>> GetTasksAsync(
        DomainTaskStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<DomainTask> query = _dbContext.Tasks.AsNoTracking();

        if (status is not null)
        {
            query = query.Where(t => t.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t =>
                EF.Functions.Like(t.Title, $"%{term}%") ||
                (t.Description != null && EF.Functions.Like(t.Description, $"%{term}%")));
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public System.Threading.Tasks.Task<DomainTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async System.Threading.Tasks.Task<DomainTask> AddAsync(DomainTask task, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        task.Id = task.Id == Guid.Empty ? Guid.NewGuid() : task.Id;
        task.CreatedAt = now;
        task.UpdatedAt = now;

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async System.Threading.Tasks.Task UpdateAsync(DomainTask task, CancellationToken cancellationToken = default)
    {
        task.UpdatedAt = DateTime.UtcNow;
        _dbContext.Tasks.Update(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task DeleteAsync(DomainTask task, CancellationToken cancellationToken = default)
    {
        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}


