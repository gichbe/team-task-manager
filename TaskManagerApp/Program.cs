using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TeamTaskManager.Model;
using TeamTaskManager.Repository;
using TeamTaskManager.Service;
using Task = TeamTaskManager.Model.Task;
using TaskStatus = TeamTaskManager.Model.TaskStatus;

namespace TeamTaskManager
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static TaskService taskService;
        static TaskAnalyzer taskAnalyzer;
        static User currentUser;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            ITaskRepository repository = new InMemoryTaskRepository();
            taskService = new TaskService(repository);
            taskAnalyzer = new TaskAnalyzer(repository);


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
                Console.WriteLine("11. Masovna promjena statusa");
                Console.WriteLine("12. Evaluacija zadatka");
                Console.WriteLine("13. Analiza performansi tima");
                Console.WriteLine("14. Sažetak zadataka po korisniku");
                Console.WriteLine("15. Predikcija rizika kašnjenja");
                Console.WriteLine("16. Izlaz");
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
                    case "11": BulkUpdateTasks(); break;
                    case "12": EvaluateTaskStatusUI(); break;
                    case "13": AnalyzeTeamPerformanceUI(); break;
                    case "14": GetUserSummaryUI(); break;
                    case "15": PredictDelayRiskUI(); break;
                    case "16": running = false; break; // ✅ Izlaz sada posljednji
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
            Console.WriteLine("NAPREDNA PRETRAGA");

            var options = new TaskSearchOptions();

            Console.Write("Tekst (Enter za preskok): ");
            var text = Console.ReadLine();
            options.Text = string.IsNullOrWhiteSpace(text) ? null : text;

            Console.Write("ID korisnika (Enter za preskok): ");
            var assignee = Console.ReadLine();
            if (int.TryParse(assignee, out int assigneeId))
                options.AssignedToUserId = assigneeId;

            Console.Write("Prioritet (1-4, Enter za preskok): ");
            var prio = Console.ReadLine();
            if (int.TryParse(prio, out int prioNum) && prioNum >= 1 && prioNum <= 4)
                options.Priority = (TaskPriority)prioNum;

            Console.Write("Samo neistekli rok? (d/n): ");
            options.OnlyNotOverdue = Console.ReadLine()?.ToLower() == "d";

            Console.WriteLine("Sortiranje: 1=rok, 2=prioritet, 3=noviji");
            var sort = Console.ReadLine();
            options.SortBy = sort switch
            {
                "1" => TaskSortOption.ByDueDateAsc,
                "2" => TaskSortOption.ByPriorityDesc,
                "3" => TaskSortOption.ByCreatedDateDesc,
                _ => TaskSortOption.None
            };

            var tasks = taskService.SearchTasks(options);
            Console.WriteLine();
            DisplayTasks(tasks);
            Console.ReadKey();
        }
        static void BulkUpdateTasks()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("       MASOVNA PROMJENA STATUSA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.Write("Unesi ID-jeve zadataka (npr. 1,2,3): ");
            var input = Console.ReadLine();

            // Parsiranje unosa
            var ids = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => int.Parse(x.Trim()))
                           .ToList();

            Console.WriteLine("\nNovi status: ");
            Console.WriteLine("1. ToDo");
            Console.WriteLine("2. U toku");
            Console.WriteLine("3. Testiranje");
            Console.WriteLine("4. Završeno");
            Console.Write("Odabir: ");
            var statusChoice = int.Parse(Console.ReadLine());

            var status = (TaskStatus)statusChoice;

            try
            {
                int updated = taskService.BulkUpdateStatus(ids, status);
                Console.WriteLine($"\n✓ Uspješno ažurirano {updated} zadataka.");
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Greška: {ex.Message}");
                Console.ResetColor();
            }
            catch (InvalidOperationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Greška: {ex.Message}");
                Console.ResetColor();
            }
            catch (KeyNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Greška: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Neočekivana greška: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nPritisni bilo koji taster za povratak...");
            Console.ReadKey();
        }

        static void ShowReport()
        {
            Console.Clear();
            Console.WriteLine("IZVJEŠTAJ\n");

            var report = taskService.GetReport();
            var users = taskService.GetAllUsers();

            Console.WriteLine($"Ukupan broj zadataka: {report.Total}\n");
            Console.WriteLine("Po statusu:");
            Console.WriteLine($"  ToDo:       {report.Todo}");
            Console.WriteLine($"  U toku:     {report.InProgress}");
            Console.WriteLine($"  Testiranje: {report.Testing}");
            Console.WriteLine($"  Završeno:   {report.Done}\n");

            Console.WriteLine("Po prioritetu:");
            Console.WriteLine($"  Nizak:      {report.Low}");
            Console.WriteLine($"  Srednji:    {report.Medium}");
            Console.WriteLine($"  Visok:      {report.High}");
            Console.WriteLine($"  Kritičan:   {report.Critical}\n");

            Console.WriteLine($"Zadaci VAN roka: {report.Overdue}\n");

            Console.WriteLine("Zadaci po korisniku (TOP 5):");
            foreach (var item in report.TasksByUser.Take(5))
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);
                Console.WriteLine($"  {(user?.Name ?? "Nepoznat")}: {item.Count}");
            }

            double progress = report.Total == 0 ? 0 : (double)report.Done / report.Total * 100.0;
            Console.WriteLine($"\nProgres projekta: {progress:0.0}% završeno");

            Console.WriteLine("\nPritisni bilo koji taster...");
            Console.ReadKey();
        }

        static void EvaluateTaskStatusUI()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("   EVALUACIJA ZADATKA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.Write("Unesi ID zadatka: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var task = taskService.GetTaskById(id);
                if (task != null)
                {
                    string status = taskAnalyzer.EvaluateTaskStatus(task);
                    Console.WriteLine($"\nRezultat evaluacije: {status}");
                }
                else Console.WriteLine("✗ Zadatak ne postoji!");
            }
            Console.ReadKey();
        }

        static void AnalyzeTeamPerformanceUI()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("   ANALIZA TIMA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            string result = taskAnalyzer.AnalyzeTeamPerformance();
            Console.WriteLine(result);
            Console.ReadKey();
        }

        static void GetUserSummaryUI()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("   SAŽETAK ZADATAKA PO KORISNIKU");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.Write("Unesi ID korisnika (1–4): ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                var summary = taskAnalyzer.GetTasksSummaryForUser(userId);
                foreach (var kv in summary)
                    Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
            Console.ReadKey();
        }

        static void PredictDelayRiskUI()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("   PREDIKCIJA RIZIKA KAŠNJENJA");
            Console.WriteLine("═══════════════════════════════════════════\n");

            Console.Write("Unesi ID zadatka: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var task = taskService.GetTaskById(id);
                if (task != null)
                {
                    string risk = taskAnalyzer.PredictDelayRisk(task);
                    Console.WriteLine($"\nRizik: {risk}");
                }
                else Console.WriteLine("✗ Zadatak ne postoji!");
            }
            Console.ReadKey();
        }


    }
}