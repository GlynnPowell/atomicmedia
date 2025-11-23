using Microsoft.EntityFrameworkCore;

namespace AtomicTasks.Infrastructure.Tasks;

using ApplicationTaskRepository = AtomicTasks.Application.Tasks.ITaskRepository;
using DomainTask = AtomicTasks.Domain.Tasks.Task;

public sealed class EfTaskRepository : ApplicationTaskRepository
{
    private readonly AtomicTasksDbContext _dbContext;

    public EfTaskRepository(AtomicTasksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<DomainTask>> GetTasksAsync(
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
        IQueryable<DomainTask> query = _dbContext.Tasks.AsNoTracking();

        if (isCompleted is not null)
        {
            query = query.Where(t => t.IsCompleted == isCompleted);
        }

        if (dueFrom is not null)
        {
            var fromDate = dueFrom.Value.Date;
            query = query.Where(t => t.DueDate >= fromDate);
        }

        if (dueTo is not null)
        {
            var toDate = dueTo.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(t => t.DueDate <= toDate);
        }

        if (!string.IsNullOrWhiteSpace(createdBy))
        {
            var createdByTerm = createdBy.Trim();
            query = query.Where(t =>
                t.CreatedBy != null && EF.Functions.Like(t.CreatedBy, $"%{createdByTerm}%"));
        }

        if (!string.IsNullOrWhiteSpace(assignedTo))
        {
            var assignedToTerm = assignedTo.Trim();
            query = query.Where(t =>
                t.AssignedTo != null && EF.Functions.Like(t.AssignedTo, $"%{assignedToTerm}%"));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t =>
                EF.Functions.Like(t.Title, $"%{term}%") ||
                (t.Description != null && EF.Functions.Like(t.Description, $"%{term}%")));
        }

        var sort = sortBy?.ToLowerInvariant();
        var direction = sortDirection?.ToLowerInvariant() == "asc" ? "asc" : "desc";

        query = (sort, direction) switch
        {
            ("title", "asc") => query.OrderBy(t => t.Title),
            ("title", "desc") => query.OrderByDescending(t => t.Title),
            ("duedate", "asc") => query.OrderBy(t => t.DueDate),
            ("duedate", "desc") => query.OrderByDescending(t => t.DueDate),
            (_, "asc") => query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 50);
        var skip = (effectivePage - 1) * effectivePageSize;

        query = query.Skip(skip).Take(effectivePageSize);

        return await query.ToListAsync(cancellationToken);
    }

    public System.Threading.Tasks.Task<DomainTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async System.Threading.Tasks.Task<DomainTask> AddAsync(DomainTask task, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
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


