using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManger.Domain.Entityies
{
    public class TaskItem
    {
        public int Id { get; private set; }

        public string Title { get; private set; }

        public string? Description { get; private set; }

        public bool IsCompleted { get; private set; }

        public DateTime? DueDate { get; private set; }

        private TaskItem()
        {
        }

        public TaskItem(
            string title,
            string? description,
            DateTime? dueDate)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            IsCompleted = false;
        }

        public void Update(
            string title,
            string? description,
            DateTime? dueDate)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
        }

        public void Complete()
        {
            IsCompleted = true;
        }

        public void SetCompletionStatus(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }
    }
}
