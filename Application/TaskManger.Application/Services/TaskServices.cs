using TaskManger.Application.DTOs;
using TaskManger.Application.Interfaces;
using TaskManger.Domain.Entityies;

namespace TaskManger.Application.Services;

public class TaskServices
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaskServices(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
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

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            DueDate = task.DueDate
        };
    }



    public async Task<TaskResponse?> GetTaskByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (task is null)
        {
            return null;
        }

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            DueDate = task.DueDate
        };
    }


    public async Task<IReadOnlyList<TaskResponse>> GetTasksAsync(
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllAsync(
            cancellationToken);

        return tasks
            .Select(task => new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate
            })
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

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            DueDate = task.DueDate
        };
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
}