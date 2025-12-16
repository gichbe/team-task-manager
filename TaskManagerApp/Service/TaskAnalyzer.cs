using System;
using System.Collections.Generic;
using System.Linq;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using TaskStatus = TeamTaskManager.Model.TaskStatus;
using Task = TeamTaskManager.Model.Task;

namespace TeamTaskManager.Service
{
    public class TaskAnalyzer
    {
        private readonly ITaskRepository repository;

        public TaskAnalyzer(ITaskRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // === 1️⃣ Evaluacija statusa zadatka ===
        public string EvaluateTaskStatus(Task task)
        {
            if (task == null) return "Invalid";

            if (task.Status == TaskStatus.Done)
                return "Completed";

            if (!task.DueDate.HasValue)
                return "No deadline";

            if (task.DueDate.Value < DateTime.Now && task.Status != TaskStatus.Done)
            {
                if (task.Priority == TaskPriority.Critical)
                    return "CRITICAL - Overdue!";
                else if (task.Priority == TaskPriority.High)
                    return "High Priority Overdue";
                else if (task.Priority == TaskPriority.Medium)
                    return "Medium Priority Overdue";
                else
                    return "Overdue";
            }
            else if (task.DueDate.Value > DateTime.Now && task.Status == TaskStatus.InProgress)
            {
                if (task.Priority == TaskPriority.High || task.Priority == TaskPriority.Critical)
                    return "On track (High)";
                else if (task.Priority == TaskPriority.Low)
                    return "On track (Low)";
                else
                    return "On track (Normal)";
            }

            return "In progress";
        }
        public string EvaluateTaskStatus_T1(Task task)
        {
            if (task == null) return "Invalid";

            if (task.Status == TaskStatus.Done)
                return "Completed";

            if (!task.DueDate.HasValue)
                return "No deadline";

            var now = DateTime.Now;
            var due = task.DueDate.Value;

            if (due < now && task.Status != TaskStatus.Done)
            {
                if (task.Priority == TaskPriority.Critical)
                    return "CRITICAL - Overdue!";
                else if (task.Priority == TaskPriority.High)
                    return "High Priority Overdue";
                else if (task.Priority == TaskPriority.Medium)
                    return "Medium Priority Overdue";
                else
                    return "Overdue";
            }
            else if (due > now && task.Status == TaskStatus.InProgress)
            {
                if (task.Priority == TaskPriority.High || task.Priority == TaskPriority.Critical)
                    return "On track (High)";
                else if (task.Priority == TaskPriority.Low)
                    return "On track (Low)";
                else
                    return "On track (Normal)";
            }

            return "In progress";
        }
        public string EvaluateTaskStatus_T2(Task task)
        {
            if (task == null) return "Invalid";

            var status = task.Status;

            if (status == TaskStatus.Done)
                return "Completed";

            if (!task.DueDate.HasValue)
                return "No deadline";

            var now = DateTime.Now;
            var due = task.DueDate.Value;
            var priority = task.Priority;

            bool isOverdue = due < now;
            bool isFutureInProgress = due > now && status == TaskStatus.InProgress;

            if (isOverdue)
            {
                if (priority == TaskPriority.Critical)
                    return "CRITICAL - Overdue!";
                else if (priority == TaskPriority.High)
                    return "High Priority Overdue";
                else if (priority == TaskPriority.Medium)
                    return "Medium Priority Overdue";
                else
                    return "Overdue";
            }
            else if (isFutureInProgress)
            {
                if (priority == TaskPriority.High || priority == TaskPriority.Critical)
                    return "On track (High)";
                else if (priority == TaskPriority.Low)
                    return "On track (Low)";
                else
                    return "On track (Normal)";
            }

            return "In progress";
        }
        private static string GetOverdueMessage1(TaskPriority priority)
        {
            if (priority == TaskPriority.Critical)
                return "CRITICAL - Overdue!";
            if (priority == TaskPriority.High)
                return "High Priority Overdue";
            if (priority == TaskPriority.Medium)
                return "Medium Priority Overdue";
            return "Overdue";
        }

        private static string GetOnTrackMessage1(TaskPriority priority)
        {
            if (priority == TaskPriority.High || priority == TaskPriority.Critical)
                return "On track (High)";
            if (priority == TaskPriority.Low)
                return "On track (Low)";
            return "On track (Normal)";
        }
        public string EvaluateTaskStatus_T3(Task task)
        {
            if (task == null) return "Invalid";

            var status = task.Status;

            if (status == TaskStatus.Done)
                return "Completed";

            if (!task.DueDate.HasValue)
                return "No deadline";

            var now = DateTime.Now;
            var due = task.DueDate.Value;
            var priority = task.Priority;

            bool isOverdue = due < now;
            bool isFutureInProgress = due > now && status == TaskStatus.InProgress;

            if (isOverdue)
                return GetOverdueMessage1(priority);

            if (isFutureInProgress)
                return GetOnTrackMessage1(priority);

            return "In progress";
        }
        public string EvaluateTaskStatus_Ref(Task task)
        {
            if (task == null) return "Invalid";

            if (task.Status == TaskStatus.Done)
                return "Completed";

            if (!task.DueDate.HasValue)
                return "No deadline";

            var now = DateTime.Now;

            if (IsOverdue(task, now))
                return GetOverdueMessage(task.Priority);

            if (IsOnTrack(task, now))
                return GetOnTrackMessage(task.Priority);

            return "In progress";
        }

        private static bool IsOverdue(Task task, DateTime now)
        {
            return task.DueDate!.Value < now && task.Status != TaskStatus.Done;
        }

        private static bool IsOnTrack(Task task, DateTime now)
        {
            return task.DueDate!.Value > now && task.Status == TaskStatus.InProgress;
        }

        private static string GetOverdueMessage(TaskPriority priority)
        {
            if (priority == TaskPriority.Critical)
                return "CRITICAL - Overdue!";
            if (priority == TaskPriority.High)
                return "High Priority Overdue";
            if (priority == TaskPriority.Medium)
                return "Medium Priority Overdue";
            return "Overdue";
        }

        private static string GetOnTrackMessage(TaskPriority priority)
        {
            if (priority == TaskPriority.High || TaskPriority.Critical == priority)
                return "On track (High)";
            if (priority == TaskPriority.Low)
                return "On track (Low)";
            return "On track (Normal)";
        }





        // === 2️⃣ Analiza performansi tima ===
        public string AnalyzeTeamPerformance()
        {
            var tasks = repository.GetAllTasks();
            if (tasks.Count == 0)
                return "Nema podataka.";

            int done = tasks.Count(t => t.Status == TaskStatus.Done);
            int inProgress = tasks.Count(t => t.Status == TaskStatus.InProgress);
            int overdue = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now && t.Status != TaskStatus.Done);

            string rating;
            if (done == 0)
                rating = "Loše";
            else if (done < tasks.Count / 3)
                rating = "Slabo";
            else if (done < tasks.Count / 2)
                rating = "Umjereno";
            else if (done < tasks.Count * 0.8)
                rating = "Dobro";
            else
                rating = "Odlično";

            if (overdue > done / 2)
                rating += " (Previše van roka)";
            else if (inProgress > done)
                rating += " (Još dosta posla)";
            else if (done == tasks.Count)
                rating += " (Sve završeno)";

            return $"Učinkovitost tima: {rating}";
        }

        // === 3️⃣ Sažetak zadataka po korisniku ===
        public Dictionary<string, int> GetTasksSummaryForUser(int userId)
        {
            var tasks = repository.GetTasksByUser(userId);
            if (tasks == null || tasks.Count == 0)
                return new Dictionary<string, int> { { "Nema zadataka", 0 } };

            var summary = new Dictionary<string, int>
            {
                { "ToDo", tasks.Count(t => t.Status == TaskStatus.ToDo) },
                { "InProgress", tasks.Count(t => t.Status == TaskStatus.InProgress) },
                { "Testing", tasks.Count(t => t.Status == TaskStatus.Testing) },
                { "Done", tasks.Count(t => t.Status == TaskStatus.Done) },
                { "Overdue", tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now && t.Status != TaskStatus.Done) }
            };

            int total = summary.Values.Sum();
            if (total > 0)
            {
                double doneRatio = (double)summary["Done"] / total;
                if (doneRatio < 0.3)
                    summary.Add("Ocjena", 1);
                else if (doneRatio < 0.6)
                    summary.Add("Ocjena", 2);
                else
                    summary.Add("Ocjena", 3);
            }

            return summary;
        }

        // === 4️⃣ Predikcija rizika kašnjenja ===
        public string PredictDelayRisk(Task task)
        {
            if (task == null) return "Nepoznat";

            int riskScore = 0;

            if (task.Priority == TaskPriority.Critical) riskScore += 3;
            else if (task.Priority == TaskPriority.High) riskScore += 2;
            else if (task.Priority == TaskPriority.Medium) riskScore += 1;

            if (!task.DueDate.HasValue)
                riskScore += 1;
            else
            {
                var daysLeft = (task.DueDate.Value - DateTime.Now).TotalDays;
                if (daysLeft < 0) riskScore += 4;
                else if (daysLeft <= 2) riskScore += 3;
                else if (daysLeft <= 5) riskScore += 2;
            }

            if (task.Status == TaskStatus.ToDo) riskScore += 2;
            else if (task.Status == TaskStatus.InProgress) riskScore += 1;

            if (riskScore >= 8)
                return "VEOMA VISOK RIZIK";
            else if (riskScore >= 5)
                return "VISOK RIZIK";
            else if (riskScore >= 3)
                return "UMJEREN RIZIK";
            else
                return "NIZAK RIZIK";
        }

       
    }
}
