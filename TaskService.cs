using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManager
{
    public class TaskService
    {
        private ITaskRepository repository;
        private List<User> users;

        public TaskService(ITaskRepository repository)
        {
            this.repository = repository;
            InitializeUsers();
        }

        private void InitializeUsers()
        {
            users = new List<User>
            {
                new User { Id = 1, Name = "Adin Mustafić", Email = "admin@test.ba", Role = UserRole.Admin },
                new User { Id = 2, Name = "Lejla Hodžić", Email = "manager@test.ba", Role = UserRole.Manager },
                new User { Id = 3, Name = "Emir Kovač", Email = "dev1@test.ba", Role = UserRole.Developer },
                new User { Id = 4, Name = "Sara Begić", Email = "dev2@test.ba", Role = UserRole.Developer }
            };
        }

        public User GetUserById(int id)
        {
            return users.FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetAllUsers()
        {
            return users.ToList();
        }

        public void CreateTask(string title, string description, TaskPriority priority,
                              int assignedTo, int createdBy, DateTime? dueDate = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Naziv zadatka ne može biti prazan");

            var task = new Task
            {
                Title = title,
                Description = description,
                Status = TaskStatus.ToDo,
                Priority = priority,
                AssignedToUserId = assignedTo,
                CreatedByUserId = createdBy,
                DueDate = dueDate
            };

            repository.AddTask(task);
        }

        public void UpdateTaskStatus(int taskId, TaskStatus newStatus)
        {
            var task = repository.GetTaskById(taskId);
            if (task == null)
                throw new ArgumentException($"Zadatak sa ID {taskId} ne postoji");

            task.Status = newStatus;
            repository.UpdateTask(task);
        }

        public List<Task> GetTasksByPriority(TaskPriority priority)
        {
            return repository.GetAllTasks()
                .Where(t => t.Priority == priority)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        public List<Task> GetOverdueTasks()
        {
            return repository.GetAllTasks()
                .Where(t => t.DueDate.HasValue &&
                           t.DueDate.Value < DateTime.Now &&
                           t.Status != TaskStatus.Done)
                .ToList();
        }

        public ITaskRepository GetRepository() => repository;
    }
}
