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
        private Dictionary<string, TaskGroup> _groups;

        public TaskHandler()
        {
            _tasks = new Dictionary<ulong, Task>();
            _groups = new Dictionary<string, TaskGroup>();
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
                f.Write(JsonConvert.SerializeObject(new Tuple<Dictionary<ulong, Task>, Dictionary<string, TaskGroup>>(_tasks, _groups)));
            }
        }
        
        public void LoadFromFile(string filename)
        {
            using (var f = File.OpenText(filename))
            {
                (_tasks, _groups) = JsonConvert.DeserializeObject<Tuple<Dictionary<ulong, Task>, Dictionary<string, TaskGroup>>>(f.ReadToEnd());
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
        private static string _taskStringFormat(Task task) => $" {task.Id,-5}|   {(task.Completed ? "x" : " ")}   | {(task.Deadline == DateTime.MinValue ? "          " : task.Deadline.ToShortDateString())} | {task.Info}";
        private const string Header = "  ID  | Done? |  Deadline  | Info\n";
        
        private string _Print(IEnumerable<Task> tasks)
        {
            var result = new StringBuilder(Header);

            foreach (var task in tasks.OrderBy(task => task.Completed))
                result.AppendLine(_taskStringFormat(task));

            return result.ToString();
        }
        
        private string _Print(IEnumerable<ulong> taskIds)
        {
            var result = new StringBuilder(Header);

            foreach (var taskId in taskIds.OrderBy(taskId => _tasks[taskId].Completed))
                result.AppendLine(_taskStringFormat(_tasks[taskId]));

            return result.ToString();
        }
        
        public string PrintAllTasks()
        {
            var result = new StringBuilder();
            var usedTasks = new HashSet<ulong>();
            
            foreach (var (name, taskGroup) in _groups)
            {
                usedTasks.UnionWith(taskGroup.Values());
                
                result.AppendFormat("Group: {0}\n", name);
                result.Append(_Print(taskGroup.Values()));
                result.AppendLine();
            }

            var notInAnyGroupTasks = new StringBuilder();

            foreach (var task in _tasks.Where(task => !usedTasks.Contains(task.Value.Id)))
                notInAnyGroupTasks.AppendLine(_taskStringFormat(task.Value));

            if (notInAnyGroupTasks.Length != 0) result.AppendLine("Not in any group:").Append(Header).Append(notInAnyGroupTasks);

            return result.ToString();
        }

        public string PrintCompleted() => _Print(_tasks.Values.Where(task => task.Completed));

        public string Today() => _Print(_tasks.Values.Where(task => task.Deadline.Date == DateTime.Today));

        public string PrintAllGroup(string name) =>
            _groups.ContainsKey(name)
                ? _Print(_groups[name].Values().Select(taskId => _tasks[taskId]))
                : "Group with this name doesn't exist!";

        public string PrintCompletedFromGroup(string name) =>
            _groups.ContainsKey(name)
                ? $"Group: {name}\n" + _Print(_groups[name].Values().Where(taskId => _tasks[taskId].Completed))
                : "Group with this name doesn't exist!";

        // Group handling
        public string Groups() => _groups.Keys.Aggregate("", (current, key) => current + (key + "\n"));

        public void CreateGroup(string name)
        {
            if (_groups.ContainsKey(name))
                Console.WriteLine("Group with this name already exists!");
            else
                _groups.Add(name, new TaskGroup());
        }

        public void DeleteGroup(string name)
        {
            if (_groups.ContainsKey(name))
                _groups.Remove(name);
            else
                Console.WriteLine("Group with this name doesn't exist!");
        }

        public void AddToGroup(ulong id, string name) => _groups[name].Add(id);

        public void DeleteFromGroup(ulong id, string name) => _groups[name].Delete(id);
    }
}