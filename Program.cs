using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using SimpleTodoApp.Models;
using SimpleTodoApp.Services;

namespace SimpleTodoApp
{
    class Program
    {
        private static TodoService _todoService = new TodoService();

        static void Main(string[] args)
        {


            Console.OutputEncoding = System.Text.Encoding.UTF8;


            Console.WriteLine("=== SIMPLE TODO APP (Refactored Version) ===\n");
            Console.WriteLine("Welcome! Todos are automatically saved.\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            RunMenu();

            Console.WriteLine("\nGoodbye! Your todos have been saved.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void RunMenu()
        {
            bool running = true;

            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddTodo(); break;
                    case "2": ViewTodos(); break;
                    case "3": ViewPending(); break;
                    case "4": ViewCompleted(); break;
                    case "5": MarkTodoComplete(); break;
                    case "6": DeleteTodo(); break;
                    case "7": SearchTodos(); break;
                    case "8": ShowStats(); break;
                    case "9": running = false; break;
                    default: Console.WriteLine("Invalid choice!"); break;
                }

                if (choice != "9")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== TODO Menu (Refactored) ===");
            Console.WriteLine("1. Add Todo");
            Console.WriteLine("2. View All Todos");
            Console.WriteLine("3. View Pending Todos");
            Console.WriteLine("4. View Completed Todos");
            Console.WriteLine("5. Mark Todo as Complete");
            Console.WriteLine("6. Delete Todo");
            Console.WriteLine("7. Search Todos");
            Console.WriteLine("8. Show Statistics");
            Console.WriteLine("9. Exit");
            Console.Write("\nChoose (1-9): ");
        }

        static void AddTodo()
        {
            Console.Clear();
            Console.WriteLine("=== ADD TODO ===\n");

            Console.WriteLine("Title: ");
            string title = Console.ReadLine();

            // Simple validation
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("\n❌ Title cannot be empty!");
                return;
            }

            Console.Write("Description (optional): ");
            string description = Console.ReadLine();

            Console.Write("Category (optional): ");
            string category = Console.ReadLine();

            Console.Write("Due date (DD/MM/YYYY or Enter for none): ");
            DateTime? dueDate = null;
            string dateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dateInput))
            {
                if (DateTime.TryParse(dateInput, out DateTime parseDate))
                {
                    dueDate = parseDate;
                }
                else
                {
                    Console.WriteLine("⚠️ Invalid date format. Using no due date.");
                }
            }

            Console.WriteLine("Priority:");
            Console.WriteLine("1. Low");
            Console.WriteLine("2. Medium (default)");
            Console.WriteLine("3. High");
            Console.WriteLine("4. Critical");
            Console.WriteLine("Select priority (1-4): ");

            Priority priority = Priority.Medium;
            string priorityInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(priorityInput))
            {
                priority = priorityInput switch
                {
                    "1" => Priority.Low,
                    "3" => Priority.High,
                    "4" => Priority.Critical,
                    _ => Priority.Medium
                };

                var todo = _todoService.AddTodo(title, description, priority, dueDate, category);

                Console.WriteLine($"\nV Todo added susccessfully! ID: {todo.Id}");
            }
        }

        static void ViewTodos()
        {
            Console.Clear();
            Console.WriteLine("=== ALL TODOS ===\n");

            var todos = _todoService.GetAllTodos();
            DisplayTodoList(todos);
        }

        static void ViewPending()
        {
            Console.Clear();
            Console.WriteLine("=== PENDING TODOS ===\n");

            var todos = _todoService.GetPendingTodos();
            DisplayTodoList(todos);
        }

        static void ViewCompleted()
        {
            Console.Clear();
            Console.WriteLine("=== COMPLETED TODOS ===\n");

            var todos = _todoService.GetCompletedTodos();
            DisplayTodoList(todos);
        }

        static void DisplayTodoList(List<TodoItem> todos)
        {
            if (todos.Count == 0)
            {
                Console.WriteLine("No todos found.");
                return;
            }

            foreach (var todo in todos)
            {
                Console.WriteLine($"ID: {todo.Id}");
                Console.WriteLine($"   {todo.GetSummary()}");

                if (!string.IsNullOrEmpty(todo.Description))
                    Console.WriteLine($" Description: {todo.Description}");

                if (!string.IsNullOrEmpty(todo.Category))
                    Console.WriteLine($" Category: {todo.Category}");

                if (todo.DueDate.HasValue)
                    Console.WriteLine($" Due: {todo.DueDate.Value:MMM dd, yyyy}");

                Console.WriteLine($"   Priority: {todo.Priority}");
                Console.WriteLine($"   Created: {todo.CreatedDate:MMM dd, yyyy}");
                Console.WriteLine();


            }

            Console.WriteLine($"Total: {todos.Count} todo(s)");

        }

        static void MarkTodoComplete()
        {
            Console.Clear();
            Console.WriteLine("=== MARK TODO COMPLETE ===\n");

            var pendingTodos = _todoService.GetPendingTodos();
            if (pendingTodos.Count == 0)
            {
                Console.WriteLine("No peending todos to complete.");
                return;
            }

            Console.WriteLine("Pending Todos");
            foreach (var todo in pendingTodos)
            {
                Console.WriteLine($"{todo.Id}: {todo.Title}");
            }


            Console.Write("\nEnter number to mark complete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (_todoService.MarkAsCompleted(id))
                {
                    Console.WriteLine($"\nV Todo {id} marked as completed!");
                }
                else
                {
                    Console.WriteLine($"\n X Todo {id} not found or already completed.");
                }
            }
            else
            {
                Console.WriteLine("\nX Invalid ID!");
            }

        }

        static void DeleteTodo()
        {
            Console.Clear();
            Console.WriteLine("=== DELETE TODO ===\n");

            var todos = _todoService.GetAllTodos();
            if (todos.Count == 0)
            {
                Console.WriteLine("No todos to delete.");
                return;
            }

            Console.WriteLine("All Todos:");
            foreach (var todo in todos) {
                {
                    Console.WriteLine($"{todo.Id}. {todo.Title}");
                }

                Console.Write("\nEnter Todo ID to delete: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    if (_todoService.DeleteTodo(id))
                    {
                        Console.WriteLine($"\nV Todo {id} deleted!");
                    }
                    else
                    {
                        Console.WriteLine($"\nX Todo {id} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("\nX Invalid ID!");

                }
            }
        }

        static void SearchTodos()
        {
            Console.Clear();
            Console.WriteLine("=== SEARCH TODOS ===\n");

            Console.WriteLine("Enter search term: ");
            string SearchTerm = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                Console.WriteLine("\nX Please enter a search term.");
                return;
            }

            var results = _todoService.SearchTodos(SearchTerm);
            Console.WriteLine($"\n Found {results.Count} result(s):");
            DisplayTodoList(results);

        }

        static void ShowStats()
        {
            Console.Clear();
            Console.WriteLine("=== STATISTICS ===\n");

            var stats = _todoService.GetStats();
            Console.WriteLine(stats.ToString());

            var overdue = _todoService.GetOverdueTodos();
            if (overdue.Count > 0)
            {
                Console.WriteLine($"\n! You have {overdue.Count} overdue todo(s):");

                foreach (var todo in overdue)
                {
                    Console.WriteLine($" . {todo.Title} (due {todo.DueDate:MMM dd})");
                }
            }

            var today = _todoService.GetTodayTodos();
            if (today.Count > 0)
            {
                Console.WriteLine($"\n You have {today.Count} todo(s) due today:");

                foreach (var todo in overdue)
                {
                    Console.WriteLine($" . {todo.Title}");
                }
            }
        }
    }
}
