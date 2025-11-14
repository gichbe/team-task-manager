using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using TeamTaskManager.Service;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TaskManagerTest
{
    [TestClass]
    public class TaskAnalyzerTests
    {
        private ITaskRepository _repo;
        private TaskAnalyzer _analyzer;

        [TestInitialize]
        public void Setup()
        {
            _repo = new InMemoryTaskRepository();

            _repo.AddTask(new Task
            {
                Id = 1,
                Title = "Zadatak 1",
                Description = "Opis 1",
                Priority = TaskPriority.Critical,
                Status = TaskStatus.InProgress,
                AssignedToUserId = 1,
                CreatedByUserId = 2,
                DueDate = DateTime.Now.AddDays(-1) // van roka
            });

            _repo.AddTask(new Task
            {
                Id = 2,
                Title = "Zadatak 2",
                Description = "Opis 2",
                Priority = TaskPriority.High,
                Status = TaskStatus.ToDo,
                AssignedToUserId = 2,
                CreatedByUserId = 1,
                DueDate = DateTime.Now.AddDays(3)
            });

            _repo.AddTask(new Task
            {
                Id = 3,
                Title = "Zadatak 3",
                Description = "Opis 3",
                Priority = TaskPriority.Medium,
                Status = TaskStatus.Done,
                AssignedToUserId = 2,
                CreatedByUserId = 1,
                DueDate = DateTime.Now.AddDays(-5)
            });

            _analyzer = new TaskAnalyzer(_repo);
        }

        // =======================
        //  TESTOVI ZA EvaluateTaskStatus
        // =======================
        [TestMethod]
        public void EvaluateTaskStatus_NullTask_ReturnsInvalid()
        {
            var result = _analyzer.EvaluateTaskStatus(null);
            Assert.AreEqual("Invalid", result);
        }

        [TestMethod]
        public void EvaluateTaskStatus_CompletedTask_ReturnsCompleted()
        {
            var task = new Task { Status = TaskStatus.Done };
            var result = _analyzer.EvaluateTaskStatus(task);
            Assert.AreEqual("Completed", result);
        }

        [TestMethod]
        public void EvaluateTaskStatus_OverdueCritical_ReturnsCriticalMessage()
        {
            var task = new Task
            {
                Priority = TaskPriority.Critical,
                Status = TaskStatus.InProgress,
                DueDate = DateTime.Now.AddDays(-2)
            };
            var result = _analyzer.EvaluateTaskStatus(task);
            Assert.AreEqual("CRITICAL - Overdue!", result);
        }

        [TestMethod]
        public void EvaluateTaskStatus_NoDeadline_ReturnsNoDeadline()
        {
            var task = new Task { Status = TaskStatus.ToDo };
            var result = _analyzer.EvaluateTaskStatus(task);
            Assert.AreEqual("No deadline", result);
        }

        [TestMethod]
        public void EvaluateTaskStatus_OnTrackHigh_ReturnsOnTrackHigh()
        {
            var task = new Task
            {
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                DueDate = DateTime.Now.AddDays(5)
            };
            var result = _analyzer.EvaluateTaskStatus(task);
            Assert.AreEqual("On track (High)", result);
        }

        // =======================
        //  TESTOVI ZA AnalyzeTeamPerformance
        // =======================
        [TestMethod]
        public void AnalyzeTeamPerformance_EmptyRepo_ReturnsNoData()
        {
            var emptyRepo = new InMemoryTaskRepository();
            var analyzer = new TaskAnalyzer(emptyRepo);
            var result = analyzer.AnalyzeTeamPerformance();
            Assert.AreEqual("Nema podataka.", result);
        }

        [TestMethod]
        public void AnalyzeTeamPerformance_NormalRepo_ReturnsText()
        {
            var result = _analyzer.AnalyzeTeamPerformance();
            StringAssert.Contains(result, "Učinkovitost tima");
        }

        // =======================
        //  TESTOVI ZA GetTasksSummaryForUser
        // =======================
        [TestMethod]
        public void GetTasksSummaryForUser_NoTasks_ReturnsZero()
        {
            var emptyRepo = new InMemoryTaskRepository();
            var analyzer = new TaskAnalyzer(emptyRepo);
            var result = analyzer.GetTasksSummaryForUser(99);
            Assert.AreEqual(0, result.Values.First());
        }

        [TestMethod]
        public void GetTasksSummaryForUser_WithTasks_ReturnsDictionaryWithValues()
        {
            var result = _analyzer.GetTasksSummaryForUser(2);
            Assert.IsTrue(result.ContainsKey("Done"));
            Assert.IsTrue(result.ContainsKey("ToDo"));
            Assert.IsTrue(result.ContainsKey("Ocjena"));
        }

        // =======================
        //  TESTOVI ZA PredictDelayRisk
        // =======================
        [TestMethod]
        public void PredictDelayRisk_NullTask_ReturnsNepoznat()
        {
            var result = _analyzer.PredictDelayRisk(null);
            Assert.AreEqual("Nepoznat", result);
        }

        [TestMethod]
        public void PredictDelayRisk_HighPrioritySoonDue_ReturnsHighRisk()
        {
            var task = new Task
            {
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress,
                DueDate = DateTime.Now.AddDays(1)
            };
            var result = _analyzer.PredictDelayRisk(task);
            StringAssert.Contains(result, "VISOK RIZIK");
        }

        [TestMethod]
        public void PredictDelayRisk_DoneOldTask_ReturnsNizakRizik()
        {
            var task = new Task
            {
                Priority = TaskPriority.Low,
                Status = TaskStatus.Done,
                DueDate = DateTime.Now.AddDays(10)
            };
            var result = _analyzer.PredictDelayRisk(task);
            Assert.AreEqual("NIZAK RIZIK", result);
        }
       



    }
}
