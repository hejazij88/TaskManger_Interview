using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories
{
    public class TaskRepository:ITaskRepository
    {
        private readonly TaskDbContext _context;

        public TaskRepository(TaskDbContext context)
        {
            _context = context;
        }


        public async Task<TaskItem?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<TaskItem>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Tasks
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            TaskItem task,
            CancellationToken cancellationToken)
        {
            await _context.Tasks.AddAsync(
                task,
                cancellationToken);
        }

        public void Update(TaskItem task)
        {
            _context.Tasks.Update(task);
        }

        public void Delete(TaskItem task)
        {
            _context.Tasks.Remove(task);
        }
    }
}
