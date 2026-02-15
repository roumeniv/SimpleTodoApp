using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using SimpleTodoApp.Models;

namespace SimpleTodoApp.Services
{
    /// <summary>
    /// Service for managing Todo items with file persistence
    /// </summary>
    public class TodoService
    {
        private readonly string _dataFilePath;
        private List<TodoItem> _todos;

        /// <summary>
        /// Create a new TodoService
        /// </summary>
        /// <param name="dataFilePath">Path to the datafile (default: todos.json)</param>
        public TodoService(string dataFilePath = "todos.json")
        { 
            _dataFilePath = dataFilePath;
            _todos = new List<TodoItem>();
            LoadTodos();
        }

        /// <summary>
        /// Get all todos
        /// </summary>
        public List<TodoItem> GetAllTodos()
        {
            return _todos.OrderBy(t => t.Priority)
                         .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                         .ToList();
        }

        /// <summary>
        /// Get a todo by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TodoItem GetTodoById(int id)
        {
            return _todos.FirstOrDefault(t => t.Id == id);
        }

        public TodoItem AddTodo(string title, string description = "",
            Priority priority=Priority.Medium, DateTime? dueDate = null,
            string category = "")
        {
            // Generate new ID
            int newId = _todos.Any() ? _todos.Max(t => t.Id) + 1 : 1;

            var todo = new TodoItem
            {
                Id = newId,
                Title = title,
                Description = description,
                Priority = priority,
                DueDate = dueDate,
                Category = category,
                CreatedDate = DateTime.Now
            };

            _todos.Add(todo);
            SaveTodos();

            return todo;
        }

        /// <summary>
        /// Update an existing todo
        /// </summary>
        public bool UpdateTodo(int id, string title, string description = "",
            Priority? priority = null, DateTime? dueDate = null,
            string category = "", bool? isCompleted = null)
        {
            var todo = GetTodoById(id);
            if (todo == null ) return false;

            todo.Title = title; 
            todo.Description = description;

            if (priority.HasValue)
                todo.Priority = priority.Value;

            todo.DueDate = dueDate;
            todo.Category = category;

            if (isCompleted != null)
                todo.IsCompleted = isCompleted.Value;

            SaveTodos();
            return true;    
        }

        /// <summary>
        /// Delete a todo
        /// </summary>
        public bool DeleteTodo(int id)
        {
            var todo = GetTodoById(id);
            if (todo == null) return false;

            _todos.Remove(todo);
            SaveTodos();
            return true;
        }
        
        /// <summary>
        /// Mark a todo as completed
        /// </summary>
        public bool MarkAsCompleted(int id)
        {
            var todo = GetTodoById(id);
            if (todo == null || todo.IsCompleted) return false;

            todo.MarkComplete();
            SaveTodos();
            return true;
        }


        /// <summary>
        /// Mark a todo as incompleted
        /// </summary>
        public bool MarkAsIncompleted(int id)
        {
            var todo = GetTodoById(id);
            if (todo == null || !todo.IsCompleted) return false;

            todo.MarkIncomplete();
            SaveTodos();
            return true;
        }

        /// <summary>
        /// Gets todos by category
        public List<TodoItem> GetTodosByCategory(string category)
        {
            return _todos.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

        }

        /// <summary>
        /// Gets pending (incomplete) todos
        /// </summary>
        public List<TodoItem> GetPendingTodos()
        {
            return _todos.Where(t => !t.IsCompleted).ToList();
        }

        /// <summary>
        /// Get completed todos
        /// </summary>
        public List<TodoItem> GetCompletedTodos()
        {
            return _todos.Where(t => t.IsCompleted).ToList();
        }

        /// <summary>
        /// Get overdue todos
        /// </summary>
        public List<TodoItem> GetOverdueTodos()
        {
            return _todos.Where(t => t.IsOverdue()).ToList();
        }

        /// <summary>
        /// Get todos due today
        /// </summary>
        public List<TodoItem> GetTodayTodos()
        {
            return _todos.Where(t => t.DueDate?.Date == DateTime.Today).ToList();
        }


        /// <summary>
        /// Get statistics about todos
        /// </summary>
        public TodoStats GetStats()
        {
            return new TodoStats
            {
                Total = _todos.Count,
                Completed = _todos.Count(T => T.IsCompleted),
                Pending = _todos.Count(t => !t.IsCompleted),
                Overdue = _todos.Count(t => t.IsOverdue())
            };
        }


        /// <summary>
        /// Search Todos by title or description
        /// </summary>
        public List<TodoItem> SearchTodos(string SearchTerm)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return new List<TodoItem>();

            return _todos.Where(t => t.Title.Contains(SearchTerm,StringComparison.OrdinalIgnoreCase) || 
                t.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase).ToList();)
        }

        /// <summary>
        /// Clears all todos (use with caution)
        /// </summary>
        public void ClearAll()
        {
            _todos.Clear();
            SaveTodos();
        }

        /// <summary>
        /// Saves todos to a file
        /// </summary>
        private void SaveTodos()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_todos, options);
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tools: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads todos from file
        /// </summary>
        private void LoadTodos()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    string json = File.ReadAllText(_dataFilePath);
                    _todos = JsonSerializer.Deserialize<List<TodoItem>>(json)
                            ?? new List<TodoItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading todos: {ex.Message}");
                _todos = new List<TodoItem>();
            }
        }

        /// <summary>
        /// Statistics about todos
        /// </summary>
        public class TodoStats
        {
            public int Total { get; set; }
            public int Completed {  get; set; }
            public int Pending { get; set; }
            public int Overdue { get; set; }

            public double CompletionPercentage => Total > 0 ? (Completed * 100.0) / Total
                          : 0;

            public override string ToString()
            {
                return $"Total: {Total} | Completed: {Completed} ({CompletionPercentage:F1}%) | " + 
                    $"Pending: {Pending} | Overdue: {Overdue}";
            }
        }

    }
}
