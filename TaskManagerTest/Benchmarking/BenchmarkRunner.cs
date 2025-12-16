using System;
using TeamTaskManager.Model;
using TeamTaskManager.Service;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

public static class BenchmarkRunner
{
    public static void Run()
    {
        var repo = new FakeTaskRepository();

        // Original + 3 optimizovane verzije (nakon što ih napraviš)
        var analyzerOriginal = new TaskAnalyzer(repo);
        var analyzerV1 = new TaskAnalyzer(repo);
        var analyzerV2 = new TaskAnalyzer(repo);
        var analyzerV3 = new TaskAnalyzer(repo);

        var testData = PrepareTestData();
        int iterations = 100000000;

        PerformanceTester.Measure("Original", analyzerOriginal.EvaluateTaskStatus, testData, iterations);

        // Kada dodaš optimizovane verzije, promijeni ove delegate:
        PerformanceTester.Measure("T1 optimized", analyzerV1.EvaluateTaskStatus_T1, testData, iterations);
        PerformanceTester.Measure("T2 optimized", analyzerV2.EvaluateTaskStatus_T2, testData, iterations);
        PerformanceTester.Measure("T3 optimized", analyzerV3.EvaluateTaskStatus_T3, testData, iterations);
    }

    private static Task[] PrepareTestData()
    {
        var now = DateTime.Now;

        return new Task[]
        {
            null,
            new Task { Status = TaskStatus.Done, Priority = TaskPriority.Medium, DueDate = now },
            new Task { Status = TaskStatus.InProgress, DueDate = null },
            new Task { Status = TaskStatus.InProgress, DueDate = now.AddDays(-1), Priority = TaskPriority.Critical },
            new Task { Status = TaskStatus.InProgress, DueDate = now.AddDays(-1), Priority = TaskPriority.High },
            new Task { Status = TaskStatus.InProgress, DueDate = now.AddDays(-1), Priority = TaskPriority.Medium },
            new Task { Status = TaskStatus.InProgress, DueDate = now.AddDays(-1), Priority = TaskPriority.Low },
            new Task { Status = TaskStatus.InProgress, DueDate = now.AddDays(1), Priority = TaskPriority.Critical },
            new Task { Status = TaskStatus.InProgress, DueDate = now.AddDays(1), Priority = TaskPriority.Low },
            new Task { Status = TaskStatus.InProgress, DueDate = now.AddDays(1), Priority = TaskPriority.Medium },
            new Task { Status = TaskStatus.ToDo, DueDate = now, Priority = TaskPriority.Medium },
        };
    }
}
