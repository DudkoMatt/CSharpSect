using System;
using System.Collections.Generic;
using System.Linq;

namespace NewcomerTask
{
    public class Task
    {
        private static ulong _nextTaskId = 1000;
        
        public string Info { get; set; }
        public bool Completed { get; set; }
        public ulong Id { get; }

        public Task(string info)
        {
            Id = _nextTaskId++;
            Info = info;
            Completed = false;
        }
        
        public Task(string info, ulong id)
        {
            Id = id;
            _nextTaskId = _nextTaskId < id ? id + 1 : _nextTaskId;
            Info = info;
            Completed = false;
        }
    }
    
    public class TaskHandler
    {
        private List<Task> _tasks;

        public TaskHandler()
        {
            _tasks = new List<Task>();
        }
        
        public void AddNewTask(string info) => _tasks.Add(new Task(info));
        
        public Task GetAt(int idx) => _tasks[idx];
        public int Length => _tasks.Count;

        public void PrintAllTasks()
        {
            Console.WriteLine("  ID  | Done? | Info");
            
            foreach (var task in _tasks.OrderBy(task => task.Completed))
                Console.WriteLine($" {task.Id, -5}|   {(task.Completed ? "x" : " ")}   | {task.Info}");
        }

        public void DeleteTask(ulong id) => _tasks.RemoveAll(task => task.Id == id);

        public void SaveToFile()
        {
            
        }

        public void LoadFromFile()
        {
            
        }

        public void MarkCompleted(ulong id)
        {
            var tmp = _tasks.Find(task => task.Id == id);
            if (tmp != null) tmp.Completed = true;
        }

        public void PrintCompleted()
        {
            Console.WriteLine("  ID  | Done? | Info");
            
            foreach (var task in _tasks.Where(task => task.Completed))
                Console.WriteLine($" {task.Id, -5}|   {(task.Completed ? "x" : " ")}   | {task.Info}");
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}