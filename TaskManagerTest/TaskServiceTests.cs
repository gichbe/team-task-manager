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

       

        [TestMethod]
        public void SearchTasks_FilterByAssignedUser()
        {
            var options = new TaskSearchOptions
            {
                AssignedToUserId = 3
            };

            var result = _taskService.SearchTasks(options);

            Assert.AreEqual(2, result.Count);
        }
        [TestMethod]
        public void SearchTasks_FilterByPriority()
        {
            var options = new TaskSearchOptions
            {
                Priority = TaskPriority.High
            };

            var result = _taskService.SearchTasks(options);

            Assert.AreEqual(2, result.Count);
        }
        [TestMethod]
        public void SearchTasks_OnlyNotOverdue_ShouldExcludeExpired()
        {
            var options = new TaskSearchOptions
            {
                OnlyNotOverdue = true
            };

            var result = _taskService.SearchTasks(options);

            // task 3 is overdue in your setup
            Assert.IsFalse(result.Any(t => t.Id == 3));
        }
        [TestMethod]
        public void SearchTasks_SortByDueDateAsc()
        {
            var options = new TaskSearchOptions
            {
                SortBy = TaskSortOption.ByDueDateAsc
            };

            var result = _taskService.SearchTasks(options);

            Assert.IsTrue(result[0].DueDate <= result[1].DueDate);
        }
        [TestMethod]
        public void SearchTasks_SortByPriorityDesc()
        {
            var options = new TaskSearchOptions
            {
                SortBy = TaskSortOption.ByPriorityDesc
            };

            var result = _taskService.SearchTasks(options);

            Assert.IsTrue(result.First().Priority >= result.Last().Priority);
        }
        [TestMethod]
        public void SearchTasks_SortByCreatedDateDesc()
        {
            var options = new TaskSearchOptions
            {
                SortBy = TaskSortOption.ByCreatedDateDesc
            };

            var result = _taskService.SearchTasks(options);

            Assert.IsTrue(result.First().CreatedDate >= result.Last().CreatedDate);
        }
        [TestMethod]
        public void BulkUpdateStatus_ValidIds_ShouldUpdateAll()
        {
            var ids = new List<int> { 1, 2 };

            int updated = _taskService.BulkUpdateStatus(ids, TaskStatus.Done);

            Assert.AreEqual(2, updated);
            Assert.IsTrue(_taskService.GetAllTasks().All(t => t.Id <= 2 ? t.Status == TaskStatus.Done : true));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BulkUpdateStatus_EmptyList_ShouldThrow()
        {
            _taskService.BulkUpdateStatus(new List<int>(), TaskStatus.Done);
        }
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void BulkUpdateStatus_IdNotFound_ShouldThrow()
        {
            _taskService.BulkUpdateStatus(new List<int> { 999 }, TaskStatus.Done);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BulkUpdateStatus_TaskAlreadyDone_ShouldThrow()
        {
            _taskService.UpdateTaskStatus(1, TaskStatus.Done);

            _taskService.BulkUpdateStatus(new List<int> { 1 }, TaskStatus.InProgress);
        }
        [TestMethod]
        public void SeedDemoData_ShouldCreateThreeTasks()
        {
            var repo = new InMemoryTaskRepository();
            var service = new TaskService(repo);

            service.SeedDemoData();

            Assert.AreEqual(3, service.GetAllTasks().Count);
        }
        [TestMethod]
        public void GetReport_ShouldReturnCorrectCounts()
        {
            var report = _taskService.GetReport();

            Assert.AreEqual(3, report.Total);
            Assert.AreEqual(2, report.Todo);
            Assert.AreEqual(1, report.InProgress);
            Assert.AreEqual(2, report.High);
        }


        
        //MOQ TESTS
        [TestMethod]
        public void CreateTask_ShouldCallRepositoryAddTask()
        {
            // Arrange
            var mockRepo = new Mock<ITaskRepository>();
            var service = new TaskService(mockRepo.Object);

            // Act
            service.CreateTask("Test", "Opis", TaskPriority.High, 3, 1);

            // Assert
            mockRepo.Verify(r => r.AddTask(It.IsAny<Task>()), Times.Once);
        }

        
        [TestMethod]
        public void UpdateTaskStatus_ShouldCallRepositoryUpdateTask()
        {
            // Arrange
            var mockRepo = new Mock<ITaskRepository>();

            mockRepo.Setup(r => r.GetTaskById(10))
                .Returns(new Task { Id = 10, Status = TaskStatus.ToDo });

            var service = new TaskService(mockRepo.Object);

            // Act
            service.UpdateTaskStatus(10, TaskStatus.Done);

            // Assert
            mockRepo.Verify(r => r.UpdateTask(It.Is<Task>(t => t.Status == TaskStatus.Done)), Times.Once);
        }

        [TestMethod]
        public void DeleteTask_WhenRepositoryReturnsFalse_ShouldReturnFalse()
        {
            // Arrange
            var mockRepo = new Mock<ITaskRepository>();

            mockRepo.Setup(r => r.DeleteTask(999)).Returns(false);

            var service = new TaskService(mockRepo.Object);

            // Act
            bool result = service.DeleteTask(999);

            // Assert
            Assert.IsFalse(result);
            mockRepo.Verify(r => r.DeleteTask(999), Times.Once);
        }

        [TestMethod]
        public void GetTasksForUser_ShouldCallRepositoryGetTasksByUser()
        {
            // Arrange
            var mockRepo = new Mock<ITaskRepository>();

            mockRepo.Setup(r => r.GetTasksByUser(3))
                .Returns(new List<Task> { new Task { Id = 1 }, new Task { Id = 2 } });

            var service = new TaskService(mockRepo.Object);

            // Act
            var result = service.GetTasksForUser(3);

            // Assert
            Assert.AreEqual(2, result.Count);
            mockRepo.Verify(r => r.GetTasksByUser(3), Times.Once);
        }
        public static IEnumerable<object[]> SearchTestData =>
    new List<object[]>
    {
        new object[] { "Task", 3 },     // sve 3 imaju "Task"
        new object[] { "1", 1 },        // samo Task 1
        new object[] { "Desc", 3 },     // sva 3 imaju opis
        new object[] { "xxx", 0 }       // ne postoji
    };

        [TestMethod]
        [DynamicData(nameof(SearchTestData), DynamicDataSourceType.Property)]
        public void SearchTasks_DynamicData_ShouldReturnCorrectResults(string text, int expectedCount)
        {
            var options = new TaskSearchOptions
            {
                Text = text,
                SortBy = TaskSortOption.None
            };

            var result = _taskService.SearchTasks(options);

            Assert.AreEqual(expectedCount, result.Count);
        }

    }
}