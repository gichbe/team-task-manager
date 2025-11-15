using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamTaskManager.Repository;
using TeamTaskManager.Service;
using TeamTaskManager.Model;
using TaskModel = TeamTaskManager.Model.Task;


namespace TaskManagerTest
{
    [TestClass]
    public class StarTests
    {
        private TaskService _service;
        private ITaskRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            _repo = new InMemoryTaskRepository();
            _repo.AddTask(new TaskModel { Id = 1, Title = "A", AssignedToUserId = 1, CreatedByUserId = 1 });
            _repo.AddTask(new TaskModel { Id = 2, Title = "B", AssignedToUserId = 1, CreatedByUserId = 1 });

            _service = new TaskService(_repo);
        }

        [TestMethod]
        public void StarTask_ShouldMarkAsStarred()
        {
            _service.StarTask(1);
            Assert.IsTrue(_service.GetTaskById(1).IsStarred);
        }

        [TestMethod]
        public void UnstarTask_ShouldRemoveStar()
        {
            _service.StarTask(1);
            _service.UnstarTask(1);
            Assert.IsFalse(_service.GetTaskById(1).IsStarred);
        }

        [TestMethod]
        public void GetStarredTasks_ShouldReturnOnlyStarred()
        {
            _service.StarTask(2);

            var result = _service.GetStarredTasks();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].Id);
        }
    }
}
