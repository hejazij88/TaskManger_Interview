using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItem>> GetAllAsync(
        CancellationToken cancellationToken);

    Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}