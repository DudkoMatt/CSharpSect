using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace NewcomerTask
{
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

        public void SaveToFile(string filename)
        {
            using (var f = File.CreateText(filename))
            {
                f.Write(JsonConvert.SerializeObject(_tasks));
            }
        }

        public void LoadFromFile(string filename)
        {
            using (var f = File.OpenText(filename))
            {
                _tasks = JsonConvert.DeserializeObject<List<Task>>(f.ReadToEnd());
            }
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
}