using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<TaskResponse> AddTaskAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken);

    Task<TaskResponse?> GetTaskByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskResponse>> GetTasksAsync(
        CancellationToken cancellationToken);

    Task<TaskResponse?> UpdateTaskAsync(
        int id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteTaskAsync(
        int id,
        CancellationToken cancellationToken);
}