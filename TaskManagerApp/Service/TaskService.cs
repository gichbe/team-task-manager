using System;
using System.Collections.Generic;
using System.Linq;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TeamTaskManager.Service
{
    public class TaskService
    {
        private readonly ITaskRepository repository;
        private readonly List<User> users;
        private readonly ICommentRepository commentRepo;

        public TaskService(ITaskRepository repo, ICommentRepository commentRepo = null)
        {
            this.repository = repo;
            this.commentRepo = commentRepo ?? new InMemoryCommentRepository();
            users = InitializeUsers();
        }
        public void AddComment(int taskId, int userId, string text)
        {
            var task = repository.GetTaskById(taskId)
                ?? throw new ArgumentException($"Zadatak {taskId} ne postoji.");

            var user = GetUserById(userId)
                ?? throw new ArgumentException($"Korisnik {userId} ne postoji.");

            var comment = new Comment
            {
                TaskId = taskId,
                UserId = userId,
                Text = text
            };

            commentRepo.AddComment(comment);
        }

        public void StarTask(int taskId)
        {
            var task = repository.GetTaskById(taskId)
                ?? throw new ArgumentException("Zadatak ne postoji.");

            task.IsStarred = true;
            repository.UpdateTask(task);
        }

        public void UnstarTask(int taskId)
        {
            var task = repository.GetTaskById(taskId)
                ?? throw new ArgumentException("Zadatak ne postoji.");

            task.IsStarred = false;
            repository.UpdateTask(task);
        }

        public List<Task> GetStarredTasks()
        {
            return repository.GetAllTasks()
                             .Where(t => t.IsStarred)
                             .ToList();
        }


        public List<Comment> GetComments(int taskId)
        {
            return commentRepo.GetCommentsForTask(taskId);
        }

        private List<User> InitializeUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Name = "Adin Mustafić", Email = "admin@test.ba", Role = UserRole.Admin },
                new User { Id = 2, Name = "Lejla Hodžić", Email = "manager@test.ba", Role = UserRole.Manager },
                new User { Id = 3, Name = "Emir Kovač", Email = "dev1@test.ba", Role = UserRole.Developer },
                new User { Id = 4, Name = "Sara Begić", Email = "dev2@test.ba", Role = UserRole.Developer }
            };
        }

        // === USERI ===
        public User GetUserById(int id) => users.FirstOrDefault(u => u.Id == id);
        public List<User> GetAllUsers() => users.ToList();

        // === TASKOVI (osnovno) ===
        public List<Task> GetAllTasks() => repository.GetAllTasks();

        public List<Task> GetTasksForUser(int userId)
            => repository.GetTasksByUser(userId);

        public List<Task> GetTasksByStatus(TaskStatus status)
            => repository.GetTasksByStatus(status);

        public List<Task> GetTasksByPriority(TaskPriority priority)
            => repository.GetAllTasks()
                         .Where(t => t.Priority == priority)
                         .OrderBy(t => t.DueDate)
                         .ToList();

        public Task GetTaskById(int id)
            => repository.GetTaskById(id);

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
            var task = repository.GetTaskById(taskId)
                       ?? throw new ArgumentException($"Zadatak sa ID {taskId} ne postoji");

            task.Status = newStatus;
            repository.UpdateTask(task);
        }

        public bool DeleteTask(int taskId)
        {
            return repository.DeleteTask(taskId);
        }

        // === VAN ROKA ===
        public List<Task> GetOverdueTasks()
        {
            return repository.GetAllTasks()
                .Where(t => t.DueDate.HasValue &&
                            t.DueDate.Value < DateTime.Now &&
                            t.Status != TaskStatus.Done)
                .ToList();
        }

        // === NAPREDNA PRETRAGA ===
        public List<Task> SearchTasks(TaskSearchOptions options)
        {
            var query = repository.GetAllTasks().AsQueryable();

            if (!string.IsNullOrWhiteSpace(options.Text))
            {
                var text = options.Text.ToLower();
                query = query.Where(t =>
                    (t.Title != null && t.Title.ToLower().Contains(text)) ||
                    (t.Description != null && t.Description.ToLower().Contains(text)));
            }

            if (options.AssignedToUserId.HasValue)
            {
                query = query.Where(t => t.AssignedToUserId == options.AssignedToUserId.Value);
            }

            if (options.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == options.Priority.Value);
            }

            if (options.OnlyNotOverdue)
            {
                query = query.Where(t => !t.DueDate.HasValue ||
                                         t.DueDate.Value >= DateTime.Now ||
                                         t.Status == TaskStatus.Done);
            }

            // sortiranje
            query = options.SortBy switch
            {
                TaskSortOption.ByDueDateAsc => query.OrderBy(t => t.DueDate ?? DateTime.MaxValue),
                TaskSortOption.ByPriorityDesc => query.OrderByDescending(t => t.Priority),
                TaskSortOption.ByCreatedDateDesc => query.OrderByDescending(t => t.CreatedDate),
                _ => query
            };

            return query.ToList();
        }

        // === IZVJEŠTAJ ===
        public TaskReport GetReport()
        {
            var tasks = repository.GetAllTasks();

            var report = new TaskReport
            {
                Total = tasks.Count,
                Todo = tasks.Count(t => t.Status == TaskStatus.ToDo),
                InProgress = tasks.Count(t => t.Status == TaskStatus.InProgress),
                Testing = tasks.Count(t => t.Status == TaskStatus.Testing),
                Done = tasks.Count(t => t.Status == TaskStatus.Done),
                Low = tasks.Count(t => t.Priority == TaskPriority.Low),
                Medium = tasks.Count(t => t.Priority == TaskPriority.Medium),
                High = tasks.Count(t => t.Priority == TaskPriority.High),
                Critical = tasks.Count(t => t.Priority == TaskPriority.Critical),
                Overdue = tasks.Count(t => t.DueDate.HasValue &&
                                           t.DueDate.Value < DateTime.Now &&
                                           t.Status != TaskStatus.Done),
                TasksByUser = tasks
                    .GroupBy(t => t.AssignedToUserId)
                    .Select(g => new TasksPerUser
                    {
                        UserId = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList()
            };

            return report;
        }
        public int BulkUpdateStatus(List<int> taskIds, TaskStatus newStatus)
        {
            if (taskIds == null || taskIds.Count == 0)
                throw new ArgumentException("Lista ID-jeva ne smije biti prazna.", nameof(taskIds));

            int updatedCount = 0;

            foreach (var id in taskIds)
            {
                var task = repository.GetTaskById(id);

                if (task == null)
                    throw new KeyNotFoundException($"Zadatak sa ID {id} ne postoji.");

                if (task.Status == TaskStatus.Done && newStatus != TaskStatus.Done)
                    throw new InvalidOperationException($"Zadatak {id} je već završen i ne može se vratiti unazad.");

                task.Status = newStatus;
                repository.UpdateTask(task);
                updatedCount++;
            }

            return updatedCount;
        }

        public void SeedDemoData()
        {
            CreateTask("Implementacija login funkcionalnosti",
                "Kreirati login formu sa validacijom",
                TaskPriority.High,
                3,
                1,
                DateTime.Now.AddDays(7));

            CreateTask("Dizajn baze podataka",
                "Kreirati ER dijagram za projekat",
                TaskPriority.Critical,
                4,
                2,
                DateTime.Now.AddDays(3));

            CreateTask("Testiranje API endpointa",
                "Unit testovi za sve API metode",
                TaskPriority.Medium,
                3,
                1,
                DateTime.Now.AddDays(-2));
        }

        public ITaskRepository GetRepository() => repository;
    }

}