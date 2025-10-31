using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamTaskManager.Model;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TeamTaskManager.Repository
{
    public interface ITaskRepository
    {
        void AddTask(Task task);
        Task GetTaskById(int id);
        List<Task> GetAllTasks();
        List<Task> GetTasksByUser(int userId);
        List<Task> GetTasksByStatus(TaskStatus status);
        void UpdateTask(Task task);
        bool DeleteTask(int id);
    }
}
