using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TeamTaskManager.Model;          
using TeamTaskManager.Repository;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TaskManagerTest
{
    [TestClass]
    public sealed class TaskRepositoryTests
    {
        private static InMemoryTaskRepository _repo;

        [ClassInitialize]
        public static void SetUp(TestContext testContext) //zahtijeva ovo TestContext testContext
        {
            _repo = new InMemoryTaskRepository();

            
            _repo.AddTask(new Task
            {
                Id = 1,
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
                Id = 2,
                Title = "Task 2",
                Description = "Desc 2",
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress,
                AssignedToUserId = 2,
                CreatedByUserId = 1,
                CreatedDate = new DateTime(2025, 10, 30)
            });
        }

        [ClassCleanup]
        public static void TearDown()
        {
            _repo = null;
        }

        [TestMethod]
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

    }
}
