using AtomicTasks.Application.Tasks;
using Moq;
using DomainTask = AtomicTasks.Domain.Tasks.Task;

namespace AtomicTasks.Tests;

public class TaskServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_MapsAndPersistsTask()
    {
        // Arrange
        var repositoryMock = new Mock<ITaskRepository>();

        DomainTask? capturedEntity = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainTask task, CancellationToken _) => task)
            .Callback<DomainTask, CancellationToken>((task, _) => capturedEntity = task);

        var service = new TaskService(repositoryMock.Object);

        var request = new CreateTaskRequest
        {
            Title = "  New task  ",
            Description = "Description",
            DueDate = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var dto = await service.CreateAsync(request);

        // Assert
        repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(capturedEntity);
        Assert.Equal("New task", capturedEntity!.Title); // trimmed
        Assert.False(capturedEntity.IsCompleted);
        Assert.Equal(capturedEntity.Title, dto.Title);
        Assert.Equal(capturedEntity.IsCompleted, dto.IsCompleted);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_UpdatesExistingTask_WhenFound()
    {
        // Arrange
        var existing = new DomainTask
        {
            Id = 1,
            Title = "Old title",
            Description = "Old description",
            IsCompleted = false
        };

        var repositoryMock = new Mock<ITaskRepository>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        repositoryMock
            .Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        var service = new TaskService(repositoryMock.Object);

        var request = new UpdateTaskRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            IsCompleted = true,
            DueDate = new DateTime(2030, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await service.UpdateAsync(existing.Id, request);

        // Assert
        Assert.NotNull(result);

        repositoryMock.Verify(
            r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Equal("Updated title", existing.Title);
        Assert.Equal("Updated description", existing.Description);
        Assert.True(existing.IsCompleted);
        Assert.Equal(request.DueDate, existing.DueDate);
    }
}


