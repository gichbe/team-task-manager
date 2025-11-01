using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamTaskManager.Model;          
using TeamTaskManager.Repository;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TaskManagerTest
{
    [TestClass]
    public sealed class TaskRepositoryTests
    {
        private InMemoryTaskRepository _repo;

        [TestInitialize]
        public  void SetUp() //zahtijeva ovo TestContext testContext
        {
            _repo = new InMemoryTaskRepository();


            _repo.AddTask(new Task
            {
                Title = "Task 1",
                Description = "Desc 1",
                Priority = TaskPriority.Low,
                Status = TaskStatus.ToDo,
                AssignedToUserId = 1,
                CreatedByUserId = 1,
                CreatedDate = new DateTime(2025, 10, 31)
            });

            _repo.AddTask(new Task
            {
                Title = "Task 2",
                Description = "Desc 2",
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress,
                AssignedToUserId = 2,
                CreatedByUserId = 1,
                CreatedDate = new DateTime(2025, 10, 30)
            });
        }


        [TestCleanup]
        public  void TearDown()
        {
            _repo = null;
        }

        [TestMethod]
        [Priority(1)]
        public void AddTask_CallTheMethod_ShouldAddTask()
        {
            var newTask = new Task
            {
                Title = "Task 3",
                Description = "Desc 3",
                Priority = TaskPriority.Medium,
                Status = TaskStatus.ToDo,
                AssignedToUserId = 3,
                CreatedByUserId = 2,
                CreatedDate = DateTime.Now
            };
            _repo.AddTask(newTask);
            var retrievedTask = _repo.GetTaskById(3);

            StringAssert.StartsWith(retrievedTask.Title, "Task");
            Assert.IsNotNull(retrievedTask);
            Assert.AreEqual("Task 3", retrievedTask.Title);
            Assert.AreEqual("Desc 3", retrievedTask.Description);
            Assert.AreEqual(TaskPriority.Medium, retrievedTask.Priority);
            Assert.AreEqual(TaskStatus.ToDo, retrievedTask.Status);
            Assert.AreEqual(3, retrievedTask.AssignedToUserId);
            Assert.AreEqual(2, retrievedTask.CreatedByUserId);
            
            CollectionAssert.Contains(_repo.GetAllTasks(), retrievedTask);
        }
        [TestMethod]
        public void AddTask_NullTask_ShouldThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _repo.AddTask(null));
        }
        [TestMethod]
        public void AddTask_MissingPriority_ShouldThrowException()
        {
            var task = new Task
            {
                Description = "Desc",
                Status = TaskStatus.ToDo,
                AssignedToUserId = 1,
                CreatedByUserId = 1,
                CreatedDate = DateTime.Now
            };
            Assert.ThrowsException<InvalidDataException>(() => _repo.AddTask(task));
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetTaskById_CallTheMethod_ShouldReturn()
        {
            var task = _repo.GetTaskById(2);


            Assert.IsNotNull(task);
            Assert.AreEqual("Task 2", task.Title);
            Assert.AreEqual("Desc 2", task.Description);
            Assert.AreEqual(TaskStatus.InProgress, task.Status);
            Assert.AreEqual(TaskPriority.High, task.Priority);
            Assert.AreEqual(2, task.AssignedToUserId);
            Assert.AreEqual(1, task.CreatedByUserId);
            Assert.AreEqual(new DateTime(2025, 10, 30), task.CreatedDate);//radi i kod datuma Equals
        }

        [TestMethod]
        public void GetTaskById_NonExistentId_ShouldReturnNull()
        {
            var task = _repo.GetTaskById(999);
            Assert.IsNull(task);
        }


        [TestMethod]
        public void GetAllTasks_CallTheMethod_ShouldReturnAllTasks()
        {
            var tasks = _repo.GetAllTasks();
            Assert.AreEqual(2, tasks.Count);
            _repo.AddTask(new Task
            {
                Title = "Task 3",
                Description = "Desc 3",
                Priority = TaskPriority.Medium,
                Status = TaskStatus.ToDo,
                AssignedToUserId = 3,
                CreatedByUserId = 2,
                CreatedDate = DateTime.Now
            });

            tasks = _repo.GetAllTasks();
            Assert.AreEqual(3, tasks.Count);
        }
        [TestMethod]
        public void GetAllTasks_EmptyRepository_ShouldReturnEmptyList()
        {
            var emptyRepo = new InMemoryTaskRepository();
            var tasks = emptyRepo.GetAllTasks();
            Assert.AreEqual(0, tasks.Count);
        }

        [TestMethod]
        public void DeleteTask_CallTheMethod_ShouldDeleteTask()
        {
            var result = _repo.DeleteTask(1);
            Assert.IsTrue(result);
            var deletedTask = _repo.GetTaskById(1);
            Assert.IsNull(deletedTask);

        }
        [TestMethod]
        public void DeleteTask_NonExistentId_ShouldReturnFalse()
        {
            var result = _repo.DeleteTask(999);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdateTask_CallTheMethod_ShouldUpdateTask()
        {
            var taskToUpdate = _repo.GetTaskById(2);
            taskToUpdate.Title = "Updated Task 2";
            taskToUpdate.Status = TaskStatus.Done;
            _repo.UpdateTask(taskToUpdate);
            var updatedTask = _repo.GetTaskById(2);
            Assert.AreEqual("Updated Task 2", taskToUpdate.Title);
            Assert.AreEqual("Desc 2", taskToUpdate.Description);
            Assert.AreEqual(TaskPriority.High, taskToUpdate.Priority);
            Assert.AreEqual(TaskStatus.Done, taskToUpdate.Status);
            Assert.AreEqual(2, taskToUpdate.AssignedToUserId);
            Assert.AreEqual(1, taskToUpdate.CreatedByUserId);
            Assert.AreEqual(new DateTime(2025, 10, 30), taskToUpdate.CreatedDate);
        }
    }
}
