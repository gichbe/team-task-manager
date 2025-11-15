using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using TeamTaskManager.Service;
using System.Linq;
using TaskModel = TeamTaskManager.Model.Task;


namespace TaskManagerTest
{
    [TestClass]
    public class CommentTests
    {
        private TaskService _service;
        private ITaskRepository _repo;
        private ICommentRepository _commentRepo;

        [TestInitialize]
        public void Setup()
        {
            _repo = new InMemoryTaskRepository();
            _commentRepo = new InMemoryCommentRepository();

            _service = new TaskService(_repo, _commentRepo);

            _repo.AddTask(new TaskModel
            {
                Id = 1,
                Title = "Test",
                AssignedToUserId = 1,
                CreatedByUserId = 1
            });
        }

        [TestMethod]
        public void AddComment_ShouldAddSuccessfully()
        {
            _service.AddComment(1, 1, "Ovo je komentar");

            var comments = _service.GetComments(1);

            Assert.AreEqual(1, comments.Count);
            Assert.AreEqual("Ovo je komentar", comments.First().Text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddComment_TaskNotFound_ShouldThrow()
        {
            _service.AddComment(99, 1, "Tekst");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddComment_EmptyText_ShouldThrow()
        {
            _service.AddComment(1, 1, "   ");
        }
    }
}
