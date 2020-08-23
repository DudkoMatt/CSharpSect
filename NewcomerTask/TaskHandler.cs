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
        public ulong AddNewTask(string info, DateTime deadline = default, ulong parent = 0)
        {
            var task = new Task(info, deadline, parent);
            _tasks.Add(task.Id, task);
            return task.Id;
        }

        public void DeleteTask(ulong id)
        {
            if (_tasks.ContainsKey(id))
                _tasks.Remove(id);
        }

        public void AddSubTask(ulong mainTaskId, string info, DateTime deadline = default)
        {
            var subTaskId = AddNewTask(info, deadline, mainTaskId);
            _tasks[mainTaskId].SubTasks.Add(subTaskId);
        }

        // File handling
        public void SaveToFile(string filename)
        {
            using var f = File.CreateText(filename);
            f.Write(JsonConvert.SerializeObject(new Tuple<Dictionary<ulong, Task>, Dictionary<string, TaskGroup>>(_tasks, _groups)));
        }
        
        public void LoadFromFile(string filename)
        {
            using var f = File.OpenText(filename);
            (_tasks, _groups) = JsonConvert.DeserializeObject<Tuple<Dictionary<ulong, Task>, Dictionary<string, TaskGroup>>>(f.ReadToEnd());
        }

        // Complete handling
        public void MarkCompleted(ulong id)
        {
            if (_tasks.ContainsKey(id)) _tasks[id].Completed = true;
        }

        // Deadline handling
        public void SetDeadline(ulong id, DateTime deadline)
        {
            if (!_tasks.ContainsKey(id)) return;
            if (_tasks[id].IsSubTask && deadline <= _tasks[_tasks[id].Parent].Deadline)
                throw new InvalidDataException("Subtask deadline cannot be further than main task deadline");
            _tasks[id].Deadline = deadline;
        }
        
        public void RemoveDeadline(ulong id) => SetDeadline(id, DateTime.MinValue);
        
        // Printing
        private const string Header = "  ID  | Done? |  Deadline  | Info\n";
        private string _taskStringFormat(Task task) =>
            $" {task.Id,-5}|" +
            $@"{(
                task.Completed 
                    ? "   x   "
                    : task.SubTasks.Count > 0 
                        ? $" {task.SubTasks.Select(taskId => _tasks[taskId]).Aggregate(0, (total, funcTask) => funcTask.Completed ? total + 1 : 0), 2}/{task.SubTasks.Count, -2} " 
                        : "       "
                )}| " +
            $"{(task.Deadline == DateTime.MinValue ? "          " : task.Deadline.ToShortDateString())} | " +
            $"{task.Info}";

        private string _PrintSubTasks(Task task, int alignment = 1)
        {
            if (task.SubTasks.Count == 0) return "";
            
            var result = new StringBuilder();
            foreach (var subTask in task.SubTasks)
            {
                result.Append(" ");
                result.Append(new string('-', alignment));
                result.AppendLine(_taskStringFormat(_tasks[subTask]));

                result.Append(_PrintSubTasks(_tasks[subTask], alignment + 1));
            }
            
            return result.ToString();
        }
        
        private string _Print(IEnumerable<Task> tasks)
        {
            var result = new StringBuilder(Header);

            foreach (var task in tasks.OrderBy(task => task.Completed))
            {
                result.AppendLine(_taskStringFormat(task));
                result.AppendLine(_PrintSubTasks(task));
            }

            return result.ToString();
        }
        
        private string _Print(IEnumerable<ulong> taskIds)
        {
            var result = new StringBuilder(Header);

            foreach (var taskId in taskIds.OrderBy(taskId => _tasks[taskId].Completed))
            {
                result.AppendLine(_taskStringFormat(_tasks[taskId]));
                result.AppendLine(_PrintSubTasks(_tasks[taskId]));
            }

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
            notInAnyGroupTasks.Append(_Print(_tasks.Where(task => !usedTasks.Contains(task.Value.Id) && !task.Value.IsSubTask).Select(task => task.Value)));

            if (notInAnyGroupTasks.Length != Header.Length) result.AppendLine("Not in any group:").Append(notInAnyGroupTasks);

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

        public void AddToGroup(ulong id, string name)
        {
            if (!_groups[name].Contains(id)) _groups[name].Add(id);
        }

        public void DeleteFromGroup(ulong id, string name) => _groups[name].Delete(id);
    }
}