using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTodoApp.Models
{
    /// <summary>
    /// Represents a single TodoItem in the application
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Unique identifier for the todo item 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title of the todo (required)
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Whether the todo is completed
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// When the todo was created
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Optional due date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Category for organization
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Priority level
        /// </summary>
        public Priority Priority { get; set; } = Priority.Medium;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TodoItem()
        {
            CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Marks the todo as completed
        /// </summary>
        public void MarkComplete()
        {
            IsCompleted = true;
        }

        /// <summary>
        /// Marks the todo as pending/ incompleted
        /// </summary>
        public void MarkIncomplete()
        {
            IsCompleted = false;
        }

        /// <summary>
        /// Check if the todo is overdue
        /// </summary>
        /// <returns></returns>
        public bool IsOverdue()
        {
            return !IsCompleted && DueDate.HasValue && DueDate.Value < DateTime.Now;
        }

        /// <summary>
        /// Gets a summary of the todo
        /// </summary>
        public string GetSummary()
        {
            string status = IsCompleted ? "V" : " ";
            string overdue = IsOverdue() ? " (OVERDUE!)" : "";
            return $"[{status}] {Title}{overdue}";
        }

        /// <summary>
        /// Validate the todo item
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Title) && Title.Length <= 100;
        }
    }

    /// <summary>
    /// Priority Levels for todos
    /// </summary>
    public enum Priority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
