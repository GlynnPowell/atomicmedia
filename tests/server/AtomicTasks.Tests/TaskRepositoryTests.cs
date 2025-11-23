using AtomicTasks.Infrastructure;
using AtomicTasks.Infrastructure.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainTask = AtomicTasks.Domain.Tasks.Task;

namespace AtomicTasks.Tests;

public class TaskRepositoryTests
{
    [Fact]
    public async System.Threading.Tasks.Task AddAndGetTasks_RoundTripsWithInMemoryDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AtomicTasksDbContext>()
            .UseInMemoryDatabase(databaseName: "tasks-tests")
            .Options;

        await using var dbContext = new AtomicTasksDbContext(options);
        var repository = new EfTaskRepository(dbContext);

        var task = new DomainTask
        {
            Title = "Write assessment",
            Description = "Implement full-stack task management app",
            IsCompleted = false
        };

        // Act
        var created = await repository.AddAsync(task);
        var tasks = await repository.GetTasksAsync(
            isCompleted: null,
            dueFrom: null,
            dueTo: null,
            createdBy: null,
            assignedTo: null,
            search: null,
            sortBy: null,
            sortDirection: null,
            page: 1,
            pageSize: 10);

        // Assert
        Assert.True(created.Id > 0);
        Assert.Single(tasks);
        Assert.Equal("Write assessment", tasks[0].Title);
    }
}


