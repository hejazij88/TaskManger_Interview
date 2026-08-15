using Moq;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Test.Services;

public class TaskServiceTests
{


    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _taskService = new TaskService(
            _taskRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddTaskAsync_Should_Create_Task()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Task one",
            Description = "Task one description",
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        _taskRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _taskService.AddTaskAsync(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            request.Title,
            result.Title);

        Assert.Equal(
            request.Description,
            result.Description);

        Assert.Equal(
            request.DueDate,
            result.DueDate);

        Assert.False(result.IsCompleted);

        _taskRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_Should_Return_Task_When_Task_Exists()
    {
        // Arrange
        var task = new TaskItem(
            "Learn EF Core",
            "Study Entity Framework Core",
            DateTime.UtcNow.AddDays(3));

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskByIdAsync(
            1,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            task.Title,
            result.Title);

        Assert.Equal(
            task.Description,
            result.Description);

        Assert.Equal(
            task.DueDate,
            result.DueDate);

        Assert.Equal(
            task.IsCompleted,
            result.IsCompleted);

        _taskRepositoryMock.Verify(
            x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetTaskByIdAsync_Should_Return_Null_When_Task_Does_Not_Exist()
    {
        // Arrange
        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _taskService.GetTaskByIdAsync(
            999,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        _taskRepositoryMock.Verify(
            x => x.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task GetTasksAsync_Should_Return_All_Tasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem(
                "Task 1",
                "Description 1",
                DateTime.UtcNow.AddDays(1)),

            new TaskItem(
                "Task 2",
                "Description 2",
                DateTime.UtcNow.AddDays(2))
        };

        _taskRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetTasksAsync(
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(2, result.Count);

        Assert.Equal(
            "Task 1",
            result[0].Title);

        Assert.Equal(
            "Task 2",
            result[1].Title);

        _taskRepositoryMock.Verify(
            x => x.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetTasksAsync_Should_Return_Empty_List_When_No_Tasks_Exist()
    {
        // Arrange
        _taskRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        var result = await _taskService.GetTasksAsync(
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _taskRepositoryMock.Verify(
            x => x.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


 

    [Fact]
    public async Task UpdateTaskAsync_Should_Update_Task()
    {
        // Arrange
        var task = new TaskItem(
            "Old Title",
            "Old Description",
            DateTime.UtcNow.AddDays(1));

        var request = new UpdateTaskRequest
        {
            Title = "New Title",
            Description = "New Description",
            IsCompleted = true,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _taskService.UpdateTaskAsync(
            1,
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "New Title",
            result.Title);

        Assert.Equal(
            "New Description",
            result.Description);

        Assert.True(result.IsCompleted);

        Assert.Equal(
            request.DueDate,
            result.DueDate);

        _taskRepositoryMock.Verify(
            x => x.Update(task),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task UpdateTaskAsync_Should_Return_Null_When_Task_Does_Not_Exist()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = "New Title",
            Description = "New Description",
            IsCompleted = true,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _taskService.UpdateTaskAsync(
            999,
            request,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        _taskRepositoryMock.Verify(
            x => x.Update(
                It.IsAny<TaskItem>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }



    [Fact]
    public async Task DeleteTaskAsync_Should_Delete_Task()
    {
        // Arrange
        var task = new TaskItem(
            "Task to Delete",
            "Description",
            DateTime.UtcNow.AddDays(1));

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _taskService.DeleteTaskAsync(
            1,
            CancellationToken.None);

        // Assert
        Assert.True(result);

        _taskRepositoryMock.Verify(
            x => x.Delete(task),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task DeleteTaskAsync_Should_Return_False_When_Task_Does_Not_Exist()
    {
        // Arrange
        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _taskService.DeleteTaskAsync(
            999,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        _taskRepositoryMock.Verify(
            x => x.Delete(
                It.IsAny<TaskItem>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }




}