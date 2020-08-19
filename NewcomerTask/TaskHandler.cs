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
        private List<Task> _tasks;

        public TaskHandler()
        {
            _tasks = new List<Task>();
        }
        
        public void AddNewTask(string info, DateTime deadline = default) => _tasks.Add(new Task(info, deadline));
        
        public Task GetAt(int idx) => _tasks[idx];
        
        public int Length => _tasks.Count;

        public string PrintAllTasks()
        {
            var result = new StringBuilder("  ID  | Done? |  Deadline  | Info\n");

            foreach (var task in _tasks.OrderBy(task => task.Completed))
                result.Append($" {task.Id, -5}|   {(task.Completed ? "x" : " ")}   | " +
                              $"{(task.Deadline == DateTime.MinValue ? "          " : task.Deadline.ToShortDateString())} | {task.Info}\n");

            return result.ToString();
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

        public string PrintCompleted()
        {
            var result = new StringBuilder("  ID  | Done? |  Deadline  | Info\n");
            
            foreach (var task in _tasks.Where(task => task.Completed))
                result.Append($" {task.Id, -5}|   {(task.Completed ? "x" : " ")}   | " +
                              $"{(task.Deadline == DateTime.MinValue ? "          " : task.Deadline.ToShortDateString())} | {task.Info}\n");

            return result.ToString();
        }

        public void SetDeadline(ulong id, DateTime deadline)
        {
            var tmp = _tasks.Find(task => task.Id == id);
            if (tmp != null) tmp.Deadline = deadline;
        }

        public void RemoveDeadline(ulong id) => SetDeadline(id, DateTime.MinValue);

        public string Today()
        {
            var result = new StringBuilder("  ID  | Done? |  Deadline  | Info\n");
            
            foreach (var task in _tasks.Where(task => task.Deadline.Date == DateTime.Today))
                result.Append($" {task.Id, -5}|   {(task.Completed ? "x" : " ")}   | " +
                              $"{(task.Deadline == DateTime.MinValue ? "          " : task.Deadline.ToShortDateString())} | {task.Info}\n");

            return result.ToString();
        }
    }
}