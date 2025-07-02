using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    public class ToDoService
    {
        private readonly string _filePath;
        private List<ToDoItem> _tasks = new();

        public ToDoService(string filePath) => _filePath = filePath;

        public async Task LoadAsync()
        {
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                _tasks = JsonSerializer.Deserialize<List<ToDoItem>>(json)
                         ?? new List<ToDoItem>();
            }
        }

        public async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        public void AddTask(string description) =>
            _tasks.Add(new ToDoItem { Description = description });

        public void RemoveTask(int idx)
        {
            if (idx >= 0 && idx < _tasks.Count)  
                _tasks.RemoveAt(idx);
        }

        public void MarkComplete(int idx)
        {
            if (idx >= 0 && idx < _tasks.Count)  
                _tasks[idx].IsCompleted = true;
        }

        public void PrintTasks()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                var t = _tasks[i];
                Console.WriteLine($"{i+1}. [{(t.IsCompleted ? 'X' : ' ')}] {t.Description}");
            }
        }
    }
}
