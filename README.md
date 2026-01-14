# Simple Todo Application

A simple console-based Todo application built with C# .NET Core.

## Features

- ✅ Add, view, edit, and delete todos
- ✅ Mark todos as complete/incomplete
- ✅ File persistence (automatic save/load)
- ✅ Clean console interface
- ✅ Input validation

## Project Structure

```
SimpleTodoApp/
├── Program.cs              # Main application entry point
├── Models/
│   └── TodoItem.cs        # Todo item data model
├── Services/
│   └── TodoService.cs     # Business logic and file operations
├── todos.txt              # Data file (auto-created)
└── README.md              # This file
```

## How to Run

1. Clone the repository
2. Open in Visual Studio 2022+
3. Press `F5` to run

## Usage

1. **Add Todo**: Enter title, optional due date and description
2. **View Todos**: See all todos with completion status
3. **Mark Complete**: Mark pending todos as completed
4. **Delete Todo**: Remove todos you no longer need
5. **Auto-save**: Todos are automatically saved to file

## Branches

- `main`: Stable version with basic features
- `refactoring`: Development branch with improved architecture

## Technologies

- C# .NET 9.0
- File I/O for persistence
- Git for version control

## Author

Roumen Ivanov
