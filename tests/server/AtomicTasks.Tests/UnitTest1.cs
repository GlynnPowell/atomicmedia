using AtomicTasks.Application.Tasks;
using DomainTask = AtomicTasks.Domain.Tasks.Task;
using DomainTaskStatus = AtomicTasks.Domain.Tasks.TaskStatus;
using DomainTaskPriority = AtomicTasks.Domain.Tasks.TaskPriority;

namespace AtomicTasks.Tests;

public class TaskMappingsTests
{
    [Fact]
    public void ToDto_MapsAllProperties()
    {
        // Arrange
        var now = new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc);

        var entity = new DomainTask
        {
            Id = Guid.NewGuid(),
            Title = "Test task",
            Description = "Description",
            Status = DomainTaskStatus.InProgress,
            Priority = DomainTaskPriority.High,
            DueDate = now.AddDays(1),
            CreatedAt = now.AddMinutes(-10),
            UpdatedAt = now
        };

        // Act
        var dto = entity.ToDto();

        // Assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Title, dto.Title);
        Assert.Equal(entity.Description, dto.Description);
        Assert.Equal(entity.Status, dto.Status);
        Assert.Equal(entity.Priority, dto.Priority);
        Assert.Equal(entity.DueDate, dto.DueDate);
        Assert.Equal(entity.CreatedAt, dto.CreatedAt);
        Assert.Equal(entity.UpdatedAt, dto.UpdatedAt);
    }
}
