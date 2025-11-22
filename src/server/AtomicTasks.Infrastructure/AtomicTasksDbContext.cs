using Microsoft.EntityFrameworkCore;

namespace AtomicTasks.Infrastructure;

using DomainTask = AtomicTasks.Domain.Tasks.Task;

public class AtomicTasksDbContext : DbContext
{
    public AtomicTasksDbContext(DbContextOptions<AtomicTasksDbContext> options)
        : base(options)
    {
    }

    public DbSet<DomainTask> Tasks => Set<DomainTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var task = modelBuilder.Entity<DomainTask>();

        task.ToTable("Tasks");

        task.HasKey(t => t.Id);

        task.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        task.Property(t => t.Description)
            .HasMaxLength(2000);

        task.Property(t => t.IsCompleted)
            .HasDefaultValue(false)
            .IsRequired();

        task.Property(t => t.CreatedAt)
            .IsRequired();

        task.Property(t => t.UpdatedAt)
            .IsRequired();
    }
}


