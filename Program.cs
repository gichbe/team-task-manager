using System;
using System.Collections.Generic;
using System.Linq;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using TeamTaskManager.Service;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TeamTaskManager
{
   
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
                Console.WriteLine("9. Napredna pretraga i sortiranje");
                Console.WriteLine("10. Izvještaj (statistika)");
                Console.WriteLine("11. Izlaz");
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
                    case "9": AdvancedSearchAndSort(); break;
                    case "10": ShowReport(); break;

                    case "11": running = false; break;

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
        static void AdvancedSearchAndSort()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("   NAPREDNA PRETRAGA I SORTIRANJE");
            Console.WriteLine("═══════════════════════════════════════════\n");

            // 1) Učitamo sve zadatke iz repozitorija
            var tasks = taskService.GetRepository().GetAllTasks();

            // 2) Filtriranje po tekstu
            Console.Write("Filtriraj po tekstu u nazivu/opisu (Enter za preskoči): ");
            string textFilter = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(textFilter))
            {
                tasks = tasks
                    .Where(t => (t.Title?.IndexOf(textFilter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                (t.Description?.IndexOf(textFilter, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            // 3) Filtriranje po korisniku kojem je dodijeljen
            Console.Write("Filtriraj po ID korisnika kojem je dodijeljen (Enter za preskoči): ");
            string assigneeInput = Console.ReadLine();
            if (int.TryParse(assigneeInput, out int assigneeId))
            {
                tasks = tasks.Where(t => t.AssignedToUserId == assigneeId).ToList();
            }

            // 4) Filtriranje po prioritetu
            Console.Write("Filtriraj po prioritetu (1=Nizak,2=Srednji,3=Visok,4=Kritičan, Enter za preskoči): ");
            string prioInput = Console.ReadLine();
            if (int.TryParse(prioInput, out int prioNum) && prioNum >= 1 && prioNum <= 4)
            {
                var prio = (TaskPriority)prioNum;
                tasks = tasks.Where(t => t.Priority == prio).ToList();
            }

            // 5) Filtriranje samo zadataka unutar roka
            Console.Write("Prikaži samo zadatke kojima rok NIJE istekao? (d/n): ");
            string onlyNotOverdue = Console.ReadLine();
            if (onlyNotOverdue?.ToLower() == "d")
            {
                tasks = tasks.Where(t => !t.DueDate.HasValue || t.DueDate.Value >= DateTime.Now || t.Status == TaskStatus.Done)
                             .ToList();
            }

            // 6) Sortiranje
            Console.WriteLine("\nSortiranje:");
            Console.WriteLine("1. Po roku (rast.)");
            Console.WriteLine("2. Po prioritetu (opadajuće)");
            Console.WriteLine("3. Po datumu kreiranja (noviji prvi)");
            Console.WriteLine("4. Bez sortiranja");
            Console.Write("Odabir: ");
            string sortChoice = Console.ReadLine();

            switch (sortChoice)
            {
                case "1":
                    tasks = tasks.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList();
                    break;
                case "2":
                    tasks = tasks.OrderByDescending(t => t.Priority).ToList();
                    break;
                case "3":
                    tasks = tasks.OrderByDescending(t => t.CreatedDate).ToList();
                    break;
            }

            Console.WriteLine();
            DisplayTasks(tasks);
            Console.ReadKey();
        }
        static void ShowReport()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("               IZVJEŠTAJ");
            Console.WriteLine("═══════════════════════════════════════════\n");

            var tasks = taskService.GetRepository().GetAllTasks();
            var users = taskService.GetAllUsers();

            int total = tasks.Count;
            int todo = tasks.Count(t => t.Status == TaskStatus.ToDo);
            int inProgress = tasks.Count(t => t.Status == TaskStatus.InProgress);
            int testing = tasks.Count(t => t.Status == TaskStatus.Testing);
            int done = tasks.Count(t => t.Status == TaskStatus.Done);

            int low = tasks.Count(t => t.Priority == TaskPriority.Low);
            int medium = tasks.Count(t => t.Priority == TaskPriority.Medium);
            int high = tasks.Count(t => t.Priority == TaskPriority.High);
            int critical = tasks.Count(t => t.Priority == TaskPriority.Critical);

            int overdue = tasks.Count(t =>
                t.DueDate.HasValue &&
                t.DueDate.Value < DateTime.Now &&
                t.Status != TaskStatus.Done
            );

            Console.WriteLine($"Ukupan broj zadataka: {total}\n");

            Console.WriteLine("Po statusu:");
            Console.WriteLine($"  ToDo:       {todo}");
            Console.WriteLine($"  U toku:     {inProgress}");
            Console.WriteLine($"  Testiranje: {testing}");
            Console.WriteLine($"  Završeno:   {done}\n");

            Console.WriteLine("Po prioritetu:");
            Console.WriteLine($"  Nizak:      {low}");
            Console.WriteLine($"  Srednji:    {medium}");
            Console.WriteLine($"  Visok:      {high}");
            Console.WriteLine($"  Kritičan:   {critical}\n");

            Console.WriteLine($"Zadaci VAN roka: {overdue}\n");

            // TOP korisnici po broju zadataka
            Console.WriteLine("Zadaci po korisniku (TOP 5):");

            var tasksByUser = tasks
                .GroupBy(t => t.AssignedToUserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            if (tasksByUser.Count == 0)
            {
                Console.WriteLine("  Nema zadataka dodijeljenih korisnicima.");
            }
            else
            {
                foreach (var item in tasksByUser)
                {
                    var user = users.FirstOrDefault(u => u.Id == item.UserId);
                    string name = user != null ? user.Name : "Nepoznat korisnik";
                    Console.WriteLine($"  {name}: {item.Count}");
                }
            }

            // Možemo dodati i "progres" - procenat završenih
            double progress = total == 0 ? 0 : (double)done / total * 100.0;
            Console.WriteLine($"\nProgres projekta: {progress:0.0}% završeno");

            Console.WriteLine("\nPritisni bilo koji taster za povratak...");
            Console.ReadKey();
        }


    }
}