using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }


    public async Task<TaskResponse> AddTaskAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = new TaskItem(
            request.Title,
            request.Description,
            request.DueDate);

        await _taskRepository.AddAsync(
            task,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(task);
    }

    public async Task<TaskResponse?> GetTaskByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            id,
            cancellationToken);

        return task is null
            ? null
            : MapToResponse(task);
    }

    public async Task<IReadOnlyList<TaskResponse>> GetTasksAsync(
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllAsync(
            cancellationToken);

        return tasks
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TaskResponse?> UpdateTaskAsync(
        int id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (task is null)
        {
            return null;
        }

        task.Update(
            request.Title,
            request.Description,
            request.DueDate);

        task.SetCompletionStatus(
            request.IsCompleted);

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(task);
    }

    public async Task<bool> DeleteTaskAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (task is null)
        {
            return false;
        }

        _taskRepository.Delete(task);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static TaskResponse MapToResponse(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            DueDate = task.DueDate
        };
    }
}