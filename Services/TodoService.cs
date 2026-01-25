using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using SimpleTodoApp.Models;

namespace SimpleTodoApp.Services
{
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
        /// <returns></returns>
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
                CreateDate = DateTime.Now
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



    }
}
