using System;
using System.Diagnostics;
using TeamTaskManager.Model;
using TeamTaskManager.Service;
using Task = TeamTaskManager.Model.Task;

public static class PerformanceTester
{
    public static void Measure(string versionName, Func<Task, string> method, Task[] testData, int iterations)
    {
        // Očisti GC radi konzistentnosti
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long memBefore = GC.GetTotalMemory(true);

        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            var task = testData[i % testData.Length];
            method(task);
        }

        sw.Stop();

        long memAfter = GC.GetTotalMemory(true);

        double memUsedMB = (memAfter - memBefore) / 1024.0 / 1024.0;

        Console.WriteLine($"{versionName}: Time = {sw.ElapsedMilliseconds} ms, MemoryΔ = {memUsedMB:F4} MB");
    }
}
