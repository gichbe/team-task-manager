using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using TeamTaskManager.Service;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TaskManagerTest
{
    [TestClass]
    public class TaskServiceTests//mora biti public, nece drugacije
    {
        private TaskService _taskService;

        [TestInitialize]
        public void Setup()
        {
            ITaskRepository _repo = new InMemoryTaskRepository();
            // Setup mock methods if needed
            _repo.AddTask(new Task
            {
                Id = 1,
                Title = "Task 1",
                Description = "Desc 1",
                Priority = TaskPriority.Low,
                Status = TaskStatus.ToDo,
                AssignedToUserId = 3,
                CreatedByUserId = 1,
                DueDate = new DateTime(2025, 11, 5)
            });

            _repo.AddTask(new Task
            {
                Id = 2,
                Title = "Task 2",
                Description = "Desc 2",
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress,
                AssignedToUserId = 4,
                CreatedByUserId = 2,
                DueDate = new DateTime(2025, 11, 3)
            });

            _repo.AddTask(new Task
            {
                Id = 3,
                Title = "Task 3",
                Description = "Desc 3",
                Priority = TaskPriority.High,
                Status = TaskStatus.ToDo,
                AssignedToUserId = 3,
                CreatedByUserId = 1,
                DueDate = new DateTime(2025, 11, 1)
            });
            _taskService = new TaskService(_repo);
        }
        [TestCleanup]
        public void Cleanup()
        {
            _taskService = null;
        }

        [TestMethod]
        public void GetAllUsers_ShouldReturnFourSeededUsers()
        {
            var users = _taskService.GetAllUsers();

            Assert.IsNotNull(users);
            Assert.AreEqual(4, users.Count, "U servisu si hardcodao 4 korisnika, to ovdje provjeravamo.");
        }

        [TestMethod]
        public void GetUserById_ExistingId_ShouldReturnUser()
        {
            var user = _taskService.GetUserById(2);

            Assert.IsNotNull(user);
            Assert.AreEqual(2, user.Id);
            Assert.AreEqual("Lejla Hodžić", user.Name);
            Assert.AreEqual(UserRole.Manager, user.Role);
        }

        [TestMethod]
        public void GetUserById_NotExistingId_ShouldReturnNull()
        {
            var user = _taskService.GetUserById(999);

            Assert.IsNull(user);
        }

        // ===== TASKOVI - GET =====

        [TestMethod]
        public void GetAllTasks_ShouldReturnAllFromRepository()
        {
            var tasks = _taskService.GetAllTasks();

            Assert.IsNotNull(tasks);
            Assert.AreEqual(3, tasks.Count);
        }

        [TestMethod]
        public void GetTasksForUser_ShouldReturnOnlyTasksForThatUser()
        {
            var tasksForUser3 = _taskService.GetTasksForUser(3);

            Assert.AreEqual(2, tasksForUser3.Count);
            Assert.IsTrue(tasksForUser3.All(t => t.AssignedToUserId == 3));
        }

        [TestMethod]
        public void GetTasksByStatus_ShouldReturnOnlyWithThatStatus()
        {
            var todoTasks = _taskService.GetTasksByStatus(TaskStatus.ToDo);

            Assert.AreEqual(2, todoTasks.Count);
            Assert.IsTrue(todoTasks.All(t => t.Status == TaskStatus.ToDo));
        }

        [TestMethod]
        public void GetTasksByPriority_ShouldReturnOrderedByDueDate()
        {
            var highPriority = _taskService.GetTasksByPriority(TaskPriority.High);

            // u SetUp imamo 2 HIGH zadatka: ID 2 (due 3.11) i ID 3 (due 1.11)
            // metoda u servisu sortira po DueDate ASC, pa bi prvi trebao biti ID 3
            Assert.AreEqual(2, highPriority.Count);
            Assert.AreEqual(3, highPriority[0].Id);   // 1.11.2025
            Assert.AreEqual(2, highPriority[1].Id);   // 3.11.2025
        }

        [TestMethod]
        public void GetTaskById_Existing_ShouldReturnTask()
        {
            var task = _taskService.GetTaskById(2);

            Assert.IsNotNull(task);
            Assert.AreEqual("Task 2", task.Title);
        }

        [TestMethod]
        public void GetTaskById_NotExisting_ShouldReturnNull()
        {
            var task = _taskService.GetTaskById(999);

            Assert.IsNull(task);
        }

        // ===== CREATE =====

        [TestMethod]
        public void CreateTask_ValidData_ShouldAddTaskToRepository()
        {
            _taskService.CreateTask(
                title: "Novi task",
                description: "Opis",
                priority: TaskPriority.Medium,
                assignedTo: 3,
                createdBy: 1,
                dueDate: new DateTime(2025, 11, 10));

            var all = _taskService.GetAllTasks();
            Assert.AreEqual(4, all.Count);

            var created = all.Last();
            Assert.AreEqual("Novi task", created.Title);
            Assert.AreEqual(TaskStatus.ToDo, created.Status, "Servis bi trebao postavljati početni status na ToDo.");
            Assert.AreEqual(TaskPriority.Medium, created.Priority);
            Assert.AreEqual(3, created.AssignedToUserId);
            Assert.AreEqual(1, created.CreatedByUserId);
            Assert.AreEqual(new DateTime(2025, 11, 10), created.DueDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateTask_EmptyTitle_ShouldThrow()
        {
            _taskService.CreateTask(
                title: "   ",
                description: "Opis",
                priority: TaskPriority.Medium,
                assignedTo: 3,
                createdBy: 1);
        }

        // ===== UPDATE STATUS =====

        [TestMethod]
        public void UpdateTaskStatus_ExistingTask_ShouldChangeStatus()
        {
            _taskService.UpdateTaskStatus(1, TaskStatus.Done);

            var updated = _taskService.GetTaskById(1);
            Assert.IsNotNull(updated);
            Assert.AreEqual(TaskStatus.Done, updated.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateTaskStatus_NotExistingTask_ShouldThrow()
        {
            _taskService.UpdateTaskStatus(999, TaskStatus.Done);
        }

        // ===== DELETE =====

        [TestMethod]
        public void DeleteTask_Existing_ShouldReturnTrueAndRemove()
        {
            var result = _taskService.DeleteTask(1);

            Assert.IsTrue(result);
            Assert.IsNull(_taskService.GetTaskById(1));
            Assert.AreEqual(2, _taskService.GetAllTasks().Count);
        }

        [TestMethod]
        public void DeleteTask_NotExisting_ShouldReturnFalse()
        {
            var result = _taskService.DeleteTask(999);

            Assert.IsFalse(result);
            Assert.AreEqual(3, _taskService.GetAllTasks().Count, "Ništa se nije obrisalo.");
        }

        [TestMethod]
        [DataRow("Task ", 3)]
        [DataRow("nepostoji", 0)]
        public void SearchTasks_ByText_DataRow(string text, int expectedCount)
        {
            

            var options = new TaskSearchOptions
            {
                Text = text,
                SortBy = TaskSortOption.None
            };

            // Act
            var result = _taskService.SearchTasks(options);

            // Assert
            Assert.AreEqual(expectedCount, result.Count);
        }



    }
}