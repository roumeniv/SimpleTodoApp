using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace SimpleTodoApp
{
    class Program
    {
        static List<TodoItem> todos = new List<TodoItem>();

        static void Main(string[] args)
        {
            // Try to enable better character support
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
            }
            catch
            {
                // If it fails, continue with default encoding
            }

            Console.WriteLine("=== SIMPLE TODO APP ===\n");

            // Load existing todos from file
            LoadTodosFromFile();

            Console.WriteLine("Welcome! This is your todo list.");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            RunMenu();

            // Optional: Save one lats time when exiting
            SaveTodosToFile();

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
                    case "1":
                        AddTodo();
                        break;
                    case "2":
                        ViewTodos();
                        break;
                    case "3":
                        DeleteTodo();
                        break;
                    case "4":
                        MarkTodoComplete();
                        break;
                    case "5":
                        EditTodo();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Press 1-6");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("\nGoodbye! Press any key to exit...");
            Console.ReadKey();
        }

        static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== TODO Menu ===");
            Console.WriteLine("1. Add Todo");
            Console.WriteLine("2. View Todos");
            Console.WriteLine("3. Delete Todo"); 
            Console.WriteLine("4. Mark Complete");
            Console.WriteLine("5. Edit Todo");
            Console.WriteLine("6. Exit");
            Console.Write("\nChoose (1-6): ");

        }

        static void AddTodo()
        {
            Console.Clear();
            Console.WriteLine("=== ADD TODO ===\n");

            Console.WriteLine("What do you need to do? ");
            string title = Console.ReadLine();

            // Simple validation
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("\n❌ Todo cannot be empty!");
                return;
            }

            // Optional: Ask for due date
            DateTime? dueDate = null;
            Console.Write("Due date (DD/MM/YYYY or Enter for none): ");
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



            // Create and add todo
            var todo = new TodoItem
            {
                Title = title,
                DueDate = dueDate,
                CreateDate = DateTime.Now
            };

            todos.Add(todo);

            // Save after adding
            SaveTodosToFile();

            Console.WriteLine($"\n✅ Added: {title}");
            if (dueDate.HasValue)
            {
                Console.WriteLine($"   Due: {dueDate.Value:MMM dd, yyyy}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewTodos()
        {
            Console.Clear();
            Console.WriteLine("=== VIEW TODOS ===\n");

            if (todos.Count == 0)
            {
                Console.WriteLine("No todos yet. Add some!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < todos.Count; i++)
            {
                var todo = todos[i];
                string status = todo.IsCompleted ? "[V]" : "[ ]";
                string number = (i + 1).ToString().PadLeft(2);

                // Add overdue indicator
                string overdue = todo.IsOverdue ? " ⚠️ OVERDUE!" : "";

                Console.WriteLine($"{number}. {status} {todo.Title}{overdue}");

                // Show due date if exists
                if (todo.DueDate.HasValue)
                {
                    string dueStatus = todo.IsOverdue ? "OVERDUE since " : "Due: ";
                    Console.WriteLine($"      {dueStatus}{todo.DueDate.Value:MMM dd, yyyy}");
                }
            }

            int completed = 0;
            foreach (var todo in todos)
            {
                if (todo.IsCompleted) completed++;
            }

            Console.WriteLine($"\n Total: {todos.Count} | Completed: {completed}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteTodo()
        {
            Console.Clear();
            Console.WriteLine("=== Delete TODO ===\n");

            if (todos.Count == 0)
            {
                Console.WriteLine("No todos to delete.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < todos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {todos[i].Title}");
            }

            Console.Write("\nEnter number to delete (0 to cancel): ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int number))
            {
                if (number == 0)
                {
                    Console.WriteLine("Cancelled.");
                }
                else
                {
                    int index = number - 1;

                    if (index >= 0 && index < todos.Count)
                    {
                        string title = todos[index].Title;
                        todos.RemoveAt(index);
                        Console.WriteLine($"\n🗑️ Deleted: {title}");

                        // Save after deleting
                        SaveTodosToFile();
                    }
                    else
                    {
                        Console.WriteLine("\nX Invalid number!");
                       
                    }
                }
            }
            else
            {
                Console.WriteLine("\nX Please enter a number!");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void MarkTodoComplete()
        {
            Console.Clear();
            Console.WriteLine("=== MARK TODO COMPLETE ===\n");

            if (todos.Count == 0)
            {
                Console.WriteLine("No todos to mark complete.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            bool hasIncomplete = false;
            for (int i = 0; i < todos.Count; i++)
            {
                if (!todos[i].IsCompleted)
                {
                    Console.WriteLine($"{i + 1}. {todos[i].Title}");
                    hasIncomplete = true;
                }
            }

            if (!hasIncomplete)
            {
                Console.WriteLine("All todos are already completed! ");
                return;
            }

            Console.Write("\nEnter number to mark complete (0 to cancel): ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int number))
            {
                if (number == 0)
                {
                    Console.WriteLine("Cancelled.");
                }
                else
                {
                    var index = number - 1;

                    if (index >= 0 && index < todos.Count)
                    {
                        if (!todos[index].IsCompleted)
                        {
                            todos[index].IsCompleted = true;
                            Console.WriteLine($"\n✅ Completed: {todos[index].Title}");

                            // Save after marking complete
                            SaveTodosToFile();
                        }
                        else
                        {
                            Console.WriteLine($"\ni Todo is already completed!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nX Invalid number!");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nX Please enter a number!");
            }


            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void EditTodo()
        {
            Console.Clear();
            Console.WriteLine("=== EDIT TODO ===");

            if (todos.Count == 0)
            {
                Console.WriteLine("No todos to edit.");
                return;
            }

            ViewTodos();

            Console.Write("\nEnter number to edit (0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int number) && number > 0)
            {
                int index = number - 1;
                if (index >= 0 && index < todos.Count)
                {

                    Console.WriteLine($"New title (current: '{todos[index].Title}'): ");
                    string newTitle = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(newTitle))
                    {
                        todos[index].Title = newTitle;
                        SaveTodosToFile();
                        Console.WriteLine($"\n✅ Update!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();

                    }
                    else
                    {
                        Console.WriteLine("\n❌ Title cannot be empty!");
                    }
                }
                else
                {
                    Console.WriteLine("\n❌ Invalid number!");
                }
            }
        }
        
        static void SaveTodosToFile()
        {
            try
            {
                // Create a simple text representation
                List<string> lines = new List<string>();

                foreach (var todo in todos)
                {
                    // Format Title|IsCompleted/CreateDate/DueDate
                    string dueDateStr = todo.DueDate?.ToString("o") ?? "";  // ISO format
                    string line = $"{todo.Title}|{todo.IsCompleted}|{todo.CreateDate:o}|{dueDateStr}";
                    lines.Add(line);
                }

                // Save to file
                File.WriteAllLines("todos.txt", lines);
                Console.WriteLine("(Saved to file)");  // Optional: show confirmation
            }
            catch (Exception ex)
            {
                // Silent failo or optional message
                Console.WriteLine($"Warning: Could not save: {ex.Message}");
            }
        }

        static void LoadTodosFromFile()
        {
            try
            {
                if (!File.Exists("todos.txt"))
                    return; // No file yet, that's OK

                string[] lines = File.ReadAllLines("todos.txt");

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');

                    if (parts.Length >= 3) // Need at least Title, IsCompleted, CreateDate
                    {
                        var todo = new TodoItem
                        {
                            Title = parts[0],
                            IsCompleted = bool.Parse(parts[1]),
                            CreateDate = DateTime.Parse(parts[2])
                        };

                        // Optional: DueDate (might be empty)
                        if (parts.Length > 3 && !string.IsNullOrEmpty(parts[3]))
                        {
                            todo.DueDate = DateTime.Parse(parts[3]);
                        }

                        todos.Add(todo);
                    }
                }

                // Optional: Show loaded count
                Console.WriteLine($"Loaded {todos.Count} todos from file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Could not load saved todos. Starting fresh.");
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }
    }
/*
    class TodoItem
    {
        public string Title { get; set; } = "";
        public bool IsCompleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DueDate { get; set; }  // Nullable DateTime

        // Optional: Add this method
        public bool IsOverdue
        {
            get
            {
                return !IsCompleted && DueDate.HasValue && DueDate.Value < DateTime.Now;
            }
        }
    }
*/
}
