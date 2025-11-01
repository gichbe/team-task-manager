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
    internal class TaskServiceTests
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




    }
}