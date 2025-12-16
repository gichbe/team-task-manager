using System.Collections.Generic;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

public class FakeTaskRepository : ITaskRepository
{
    public void AddTask(Task task) { }
    public bool DeleteTask(int id) => false;
    public List<Task> GetAllTasks() => new List<Task>();
    public Task GetTaskById(int id) => null;
    public List<Task> GetTasksByStatus(TaskStatus status) => new List<Task>();
    public List<Task> GetTasksByUser(int userId) => new List<Task>();
    public void UpdateTask(Task task) { }
}
