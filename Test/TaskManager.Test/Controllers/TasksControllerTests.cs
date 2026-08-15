using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Test.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;

    private readonly Mock<IValidator<CreateTaskRequest>>
        _createValidatorMock;

    private readonly Mock<IValidator<UpdateTaskRequest>>
        _updateValidatorMock;

    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();

        _createValidatorMock =
            new Mock<IValidator<CreateTaskRequest>>();

        _updateValidatorMock =
            new Mock<IValidator<UpdateTaskRequest>>();

        _controller = new TasksController(
            _taskServiceMock.Object);
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_When_Request_Is_Valid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Learn ASP.NET Core",
            Description = "Study Web API",
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        var response = new TaskResponse
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
            IsCompleted = false,
            DueDate = request.DueDate
        };

        _createValidatorMock
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _taskServiceMock
            .Setup(x => x.AddTaskAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(
            request,
            CancellationToken.None);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(
            nameof(_controller.GetById),
            createdResult.ActionName);

        Assert.Equal(
            response,
            createdResult.Value);

        Assert.Equal(
            1,
            createdResult.RouteValues!["id"]);

        _taskServiceMock.Verify(
            x => x.AddTaskAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Request_Is_Invalid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "",
            Description = "Test"
        };

        var validationResult = new ValidationResult(
            new[]
            {
                new ValidationFailure(
                    nameof(CreateTaskRequest.Title),
                    "Title is required.")
            });

        _createValidatorMock
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Create(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);

        _taskServiceMock.Verify(
            x => x.AddTaskAsync(
                It.IsAny<CreateTaskRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Tasks()
    {
        // Arrange
        var tasks = new List<TaskResponse>
        {
            new()
            {
                Id = 1,
                Title = "Task 1",
                Description = "Description 1",
                IsCompleted = false,
                DueDate = DateTime.UtcNow.AddDays(1)
            },

            new()
            {
                Id = 2,
                Title = "Task 2",
                Description = "Description 2",
                IsCompleted = true,
                DueDate = DateTime.UtcNow.AddDays(2)
            }
        };

        _taskServiceMock
            .Setup(x => x.GetTasksAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAll(
            CancellationToken.None);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result);

        Assert.Equal(
            tasks,
            okResult.Value);

        _taskServiceMock.Verify(
            x => x.GetTasksAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetById_Should_Return_Ok_When_Task_Exists()
    {
        // Arrange
        var response = new TaskResponse
        {
            Id = 1,
            Title = "Learn C#",
            Description = "Study C#",
            IsCompleted = false,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        _taskServiceMock
            .Setup(x => x.GetTaskByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(
            1,
            CancellationToken.None);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result);

        Assert.Equal(
            response,
            okResult.Value);

        _taskServiceMock.Verify(
            x => x.GetTaskByIdAsync(
                1,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        // Arrange
        _taskServiceMock
            .Setup(x => x.GetTaskByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskResponse?)null);

        // Act
        var result = await _controller.GetById(
            999,
            CancellationToken.None);

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(result);

        Assert.NotNull(notFoundResult.Value);

        _taskServiceMock.Verify(
            x => x.GetTaskByIdAsync(
                999,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Update_Should_Return_Ok_When_Task_Is_Updated()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = "Updated Task",
            Description = "Updated Description",
            IsCompleted = true,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        var response = new TaskResponse
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
            IsCompleted = true,
            DueDate = request.DueDate
        };

        _updateValidatorMock
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _taskServiceMock
            .Setup(x => x.UpdateTaskAsync(
                1,
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Update(
            1,
            request,
            CancellationToken.None);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result);

        Assert.Equal(
            response,
            okResult.Value);

        _taskServiceMock.Verify(
            x => x.UpdateTaskAsync(
                1,
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Request_Is_Invalid()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = ""
        };

        var validationResult = new ValidationResult(
            new[]
            {
                new ValidationFailure(
                    nameof(UpdateTaskRequest.Title),
                    "Title is required.")
            });

        _updateValidatorMock
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Update(
            1,
            request,
            CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);

        _taskServiceMock.Verify(
            x => x.UpdateTaskAsync(
                It.IsAny<int>(),
                It.IsAny<UpdateTaskRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task Update_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = "Updated Task",
            Description = "Updated Description",
            IsCompleted = true,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        _updateValidatorMock
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _taskServiceMock
            .Setup(x => x.UpdateTaskAsync(
                999,
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskResponse?)null);

        // Act
        var result = await _controller.Update(
            999,
            request,
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);

        _taskServiceMock.Verify(
            x => x.UpdateTaskAsync(
                999,
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    
    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Task_Is_Deleted()
    {
        // Arrange
        _taskServiceMock
            .Setup(x => x.DeleteTaskAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(
            1,
            CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _taskServiceMock.Verify(
            x => x.DeleteTaskAsync(
                1,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        // Arrange
        _taskServiceMock
            .Setup(x => x.DeleteTaskAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(
            999,
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);

        _taskServiceMock.Verify(
            x => x.DeleteTaskAsync(
                999,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}