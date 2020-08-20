using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace NewcomerTask
{
    public class TaskHandler
    {
        private Dictionary<ulong, Task> _tasks;

        public TaskHandler()
        {
            _tasks = new Dictionary<ulong, Task>();
        }

        // Creation/removal handling
        public void AddNewTask(string info, DateTime deadline = default)
        {
            var task = new Task(info, deadline);
            _tasks.Add(task.Id, task);
        }

        public void DeleteTask(ulong id)
        {
            if (_tasks.ContainsKey(id))
                _tasks.Remove(id);
        }

        // File handling
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
                _tasks = JsonConvert.DeserializeObject<Dictionary<ulong, Task>>(f.ReadToEnd());
            }
        }

        // Complete handling
        public void MarkCompleted(ulong id)
        {
            if (_tasks.ContainsKey(id)) _tasks[id].Completed = true;
        }

        // Deadline handling
        public void SetDeadline(ulong id, DateTime deadline)
        {
            if (_tasks.ContainsKey(id)) _tasks[id].Deadline = deadline;
        }
        
        public void RemoveDeadline(ulong id) => SetDeadline(id, DateTime.MinValue);
        
        // Printing
        private static string _Print(IEnumerable<Task> tasks)
        {
            var result = new StringBuilder("  ID  | Done? |  Deadline  | Info\n");

            foreach (var task in tasks)
                result.Append($" {task.Id, -5}|   {(task.Completed ? "x" : " ")}   | " +
                              $"{(task.Deadline == DateTime.MinValue ? "          " : task.Deadline.ToShortDateString())} | {task.Info}\n");

            return result.ToString();
        }
        
        public string PrintAllTasks() => _Print(_tasks.Values.OrderBy(task => task.Completed));

        public string PrintCompleted() => _Print(_tasks.Values.Where(task => task.Completed));

        public string Today() => _Print(_tasks.Values.Where(task => task.Deadline.Date == DateTime.Today).OrderBy(task => task.Completed));
    }
}