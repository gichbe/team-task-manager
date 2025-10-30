using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManager
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private List<Task> tasks = new List<Task>();
        private int nextId = 1;

        public void AddTask(Task task)
        {
            task.Id = nextId++;
            task.CreatedDate = DateTime.Now;
            tasks.Add(task);
        }

        public Task GetTaskById(int id)
        {
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        public List<Task> GetAllTasks()
        {
            return tasks.ToList();
        }

        public List<Task> GetTasksByUser(int userId)
        {
            return tasks.Where(t => t.AssignedToUserId == userId).ToList();
        }

        public List<Task> GetTasksByStatus(TaskStatus status)
        {
            return tasks.Where(t => t.Status == status).ToList();
        }

        public void UpdateTask(Task task)
        {
            var existing = GetTaskById(task.Id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.Status = task.Status;
                existing.Priority = task.Priority;
                existing.DueDate = task.DueDate;
                existing.AssignedToUserId = task.AssignedToUserId;
            }
        }

        public bool DeleteTask(int id)
        {
            var task = GetTaskById(id);
            if (task != null)
            {
                tasks.Remove(task);
                return true;
            }
            return false;
        }
    }
}
