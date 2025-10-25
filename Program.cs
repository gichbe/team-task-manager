using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamTaskManager
{
    // Model klase
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int AssignedToUserId { get; set; }
        public int CreatedByUserId { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }

    public enum TaskStatus
    {
        ToDo = 1,
        InProgress = 2,
        Testing = 3,
        Done = 4
    }

    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        Developer = 3
    }

    // Interfejs za repozitorij
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

    // Konkretna implementacija repozitorija
    public class InMemoryTaskRepository : ITaskRepository
    {
        private List<Task> tasks = new List<Task>();
        private int nextId = 1;

        public void AddTask(Task task)
        {
            task.Id = nextId++;
            task.CreatedDate = DateTime.Now;
            tasks.Add(task);
        }

        public Task GetTaskById(int id)
        {
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        public List<Task> GetAllTasks()
        {
            return tasks.ToList();
        }

        public List<Task> GetTasksByUser(int userId)
        {
            return tasks.Where(t => t.AssignedToUserId == userId).ToList();
        }

        public List<Task> GetTasksByStatus(TaskStatus status)
        {
            return tasks.Where(t => t.Status == status).ToList();
        }

        public void UpdateTask(Task task)
        {
            var existing = GetTaskById(task.Id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.Status = task.Status;
                existing.Priority = task.Priority;
                existing.DueDate = task.DueDate;
                existing.AssignedToUserId = task.AssignedToUserId;
            }
        }

        public bool DeleteTask(int id)
        {
            var task = GetTaskById(id);
            if (task != null)
            {
                tasks.Remove(task);
                return true;
            }
            return false;
        }
    }

    // Servisni sloj sa poslovnom logikom
    public class TaskService
    {
        private ITaskRepository repository;
        private List<User> users;

        public TaskService(ITaskRepository repository)
        {
            this.repository = repository;
            InitializeUsers();
        }

        private void InitializeUsers()
        {
            users = new List<User>
            {
                new User { Id = 1, Name = "Adin Mustafić", Email = "admin@test.ba", Role = UserRole.Admin },
                new User { Id = 2, Name = "Lejla Hodžić", Email = "manager@test.ba", Role = UserRole.Manager },
                new User { Id = 3, Name = "Emir Kovač", Email = "dev1@test.ba", Role = UserRole.Developer },
                new User { Id = 4, Name = "Sara Begić", Email = "dev2@test.ba", Role = UserRole.Developer }
            };
        }

        public User GetUserById(int id)
        {
            return users.FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetAllUsers()
        {
            return users.ToList();
        }

        public void CreateTask(string title, string description, TaskPriority priority, 
                              int assignedTo, int createdBy, DateTime? dueDate = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Naziv zadatka ne može biti prazan");

            var task = new Task
            {
                Title = title,
                Description = description,
                Status = TaskStatus.ToDo,
                Priority = priority,
                AssignedToUserId = assignedTo,
                CreatedByUserId = createdBy,
                DueDate = dueDate
            };

            repository.AddTask(task);
        }

        public void UpdateTaskStatus(int taskId, TaskStatus newStatus)
        {
            var task = repository.GetTaskById(taskId);
            if (task == null)
                throw new ArgumentException($"Zadatak sa ID {taskId} ne postoji");

            task.Status = newStatus;
            repository.UpdateTask(task);
        }

        public List<Task> GetTasksByPriority(TaskPriority priority)
        {
            return repository.GetAllTasks()
                .Where(t => t.Priority == priority)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        public List<Task> GetOverdueTasks()
        {
            return repository.GetAllTasks()
                .Where(t => t.DueDate.HasValue && 
                           t.DueDate.Value < DateTime.Now && 
                           t.Status != TaskStatus.Done)
                .ToList();
        }

        public ITaskRepository GetRepository() => repository;
    }

    // Glavna konzolna aplikacija
    class Program
    {
        static TaskService taskService;
        static User currentUser;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            ITaskRepository repository = new InMemoryTaskRepository();
            taskService = new TaskService(repository);

            // Demo podatci
            InitializeDemoData();

            // Login
            Login();

            // Glavni meni
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("═══════════════════════════════════════════");
                Console.WriteLine("       TEAM TASK MANAGER");
                Console.WriteLine($"       Korisnik: {currentUser.Name}");
                Console.WriteLine("═══════════════════════════════════════════\n");
                Console.WriteLine("1. Kreiraj novi zadatak");
                Console.WriteLine("2. Pregledaj sve zadatke");
                Console.WriteLine("3. Pregledaj moje zadatke");
                Console.WriteLine("4. Pretraži zadatke po statusu");
                Console.WriteLine("5. Izmijeni status zadatka");
                Console.WriteLine("6. Obriši zadatak");
                Console.WriteLine("7. Pregled zadataka po prioritetu");
                Console.WriteLine("8. Pregled zadataka van roka");
                Console.WriteLine("9. Izlaz");
                Console.Write("\nOdabir: ");

                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1": CreateNewTask(); break;
                    case "2": ViewAllTasks(); break;
                    case "3": ViewMyTasks(); break;
                    case "4": SearchTasksByStatus(); break;
                    case "5": UpdateTaskStatus(); break;
                    case "6": DeleteTask(); break;
                    case "7": ViewTasksByPriority(); break;
                    case "8": ViewOverdueTasks(); break;
                    case "9": running = false; break;
                    default: 
                        Console.WriteLine("Neispravan unos!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void Login()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("           LOGIN");
            Console.WriteLine("═══════════════════════════════════════════\n");
            
            var users = taskService.GetAllUsers();
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {users[i].Name} - {users[i].Role}");
            }
            
            Console.Write("\nOdaberi korisnika (1-4): ");
            if (int.TryParse(Console.ReadLine(), out int userId) && userId >= 1 && userId <= 4)
            {
                currentUser = taskService.GetUserById(userId);
            }
            else
            {
                currentUser = taskService.GetUserById(1);
            }
        }

        static void InitializeDemoData()
        {
            taskService.CreateTask(
                "Implementacija login funkcionalnosti",
                "Kreirati login formu sa validacijom",
                TaskPriority.High,
                3,
                1,
                DateTime.Now.AddDays(7)
            );

            taskService.CreateTask(
                "Dizajn baze podataka",
                "Kreirati ER dijagram za projekat",
                TaskPriority.Critical,
                4,
                2,
                DateTime.Now.AddDays(3)
            );

            taskService.CreateTask(
                "Testiranje API endpointa",
                "Unit testovi za sve API metode",
                TaskPriority.Medium,
                3,
                1,
                DateTime.Now.AddDays(-2)
            );
        }

        static void CreateNewTask()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       KREIRANJE NOVOG ZADATKA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.Write("Naziv zadatka: ");
            string title = Console.ReadLine();

            Console.Write("Opis zadatka: ");
            string description = Console.ReadLine();

            Console.WriteLine("\nPrioritet:");
            Console.WriteLine("1. Nizak");
            Console.WriteLine("2. Srednji");
            Console.WriteLine("3. Visok");
            Console.WriteLine("4. Kritičan");
            Console.Write("Odabir: ");
            int priorityChoice = int.Parse(Console.ReadLine());

            Console.WriteLine("\nDodijeli korisniku:");
            var users = taskService.GetAllUsers();
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{users[i].Id}. {users[i].Name}");
            }
            Console.Write("Odabir: ");
            int assignedTo = int.Parse(Console.ReadLine());

            Console.Write("\nRok (broj dana, Enter za bez roka): ");
            string daysInput = Console.ReadLine();
            DateTime? dueDate = null;
            if (!string.IsNullOrWhiteSpace(daysInput) && int.TryParse(daysInput, out int days))
            {
                dueDate = DateTime.Now.AddDays(days);
            }

            try
            {
                taskService.CreateTask(title, description, (TaskPriority)priorityChoice, 
                                      assignedTo, currentUser.Id, dueDate);
                Console.WriteLine("\n✓ Zadatak uspješno kreiran!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Greška: {ex.Message}");
            }

            Console.ReadKey();
        }

        static void ViewAllTasks()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       SVI ZADACI");
            Console.WriteLine("═══════════════════════════════════════════\n");

            var tasks = taskService.GetRepository().GetAllTasks();
            DisplayTasks(tasks);
            
            Console.ReadKey();
        }

        static void ViewMyTasks()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       MOJI ZADACI");
            Console.WriteLine("═══════════════════════════════════════════\n");

            var tasks = taskService.GetRepository().GetTasksByUser(currentUser.Id);
            DisplayTasks(tasks);
            
            Console.ReadKey();
        }

        static void SearchTasksByStatus()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       PRETRAGA PO STATUSU");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.WriteLine("1. ToDo");
            Console.WriteLine("2. U toku");
            Console.WriteLine("3. Testiranje");
            Console.WriteLine("4. Završeno");
            Console.Write("\nOdabir: ");
            
            if (int.TryParse(Console.ReadLine(), out int statusChoice) && 
                statusChoice >= 1 && statusChoice <= 4)
            {
                var tasks = taskService.GetRepository()
                    .GetTasksByStatus((TaskStatus)statusChoice);
                Console.WriteLine();
                DisplayTasks(tasks);
            }
            
            Console.ReadKey();
        }

        static void UpdateTaskStatus()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       IZMJENA STATUSA ZADATKA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.Write("ID zadatka: ");
            if (int.TryParse(Console.ReadLine(), out int taskId))
            {
                var task = taskService.GetRepository().GetTaskById(taskId);
                if (task != null)
                {
                    Console.WriteLine($"\nTrenutni status: {task.Status}");
                    Console.WriteLine("\nNovi status:");
                    Console.WriteLine("1. ToDo");
                    Console.WriteLine("2. U toku");
                    Console.WriteLine("3. Testiranje");
                    Console.WriteLine("4. Završeno");
                    Console.Write("\nOdabir: ");
                    
                    if (int.TryParse(Console.ReadLine(), out int newStatus) && 
                        newStatus >= 1 && newStatus <= 4)
                    {
                        taskService.UpdateTaskStatus(taskId, (TaskStatus)newStatus);
                        Console.WriteLine("\n✓ Status uspješno ažuriran!");
                    }
                }
                else
                {
                    Console.WriteLine("\n✗ Zadatak ne postoji!");
                }
            }
            
            Console.ReadKey();
        }

        static void DeleteTask()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       BRISANJE ZADATKA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.Write("ID zadatka za brisanje: ");
            if (int.TryParse(Console.ReadLine(), out int taskId))
            {
                if (taskService.GetRepository().DeleteTask(taskId))
                {
                    Console.WriteLine("\n✓ Zadatak uspješno obrisan!");
                }
                else
                {
                    Console.WriteLine("\n✗ Zadatak ne postoji!");
                }
            }
            
            Console.ReadKey();
        }

        static void ViewTasksByPriority()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       ZADACI PO PRIORITETU");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.WriteLine("1. Nizak");
            Console.WriteLine("2. Srednji");
            Console.WriteLine("3. Visok");
            Console.WriteLine("4. Kritičan");
            Console.Write("\nOdabir: ");
            
            if (int.TryParse(Console.ReadLine(), out int priority) && 
                priority >= 1 && priority <= 4)
            {
                var tasks = taskService.GetTasksByPriority((TaskPriority)priority);
                Console.WriteLine();
                DisplayTasks(tasks);
            }
            
            Console.ReadKey();
        }

        static void ViewOverdueTasks()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       ZADACI VAN ROKA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            var tasks = taskService.GetOverdueTasks();
            DisplayTasks(tasks);
            
            Console.ReadKey();
        }

        static void DisplayTasks(List<Task> tasks)
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("Nema zadataka za prikaz.");
                return;
            }

            foreach (var task in tasks)
            {
                var assignedUser = taskService.GetUserById(task.AssignedToUserId);
                Console.WriteLine($"───────────────────────────────────────────");
                Console.WriteLine($"ID: {task.Id}");
                Console.WriteLine($"Naziv: {task.Title}");
                Console.WriteLine($"Opis: {task.Description}");
                Console.WriteLine($"Status: {task.Status}");
                Console.WriteLine($"Prioritet: {task.Priority}");
                Console.WriteLine($"Dodijeljen: {assignedUser?.Name ?? "N/A"}");
                Console.WriteLine($"Kreirano: {task.CreatedDate:dd.MM.yyyy}");
                if (task.DueDate.HasValue)
                {
                    Console.WriteLine($"Rok: {task.DueDate.Value:dd.MM.yyyy}");
                    if (task.DueDate.Value < DateTime.Now && task.Status != TaskStatus.Done)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("⚠ VAN ROKA!");
                        Console.ResetColor();
                    }
                }
            }
            Console.WriteLine($"───────────────────────────────────────────");
        }
    }
}