using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NewcomerTask
{
    public class Task
    {
        private static ulong _nextTaskId = 1000;
        
        public string Info { get; set; }
        public bool Completed { get; set; }
        public ulong Id { get; }
        public DateTime Deadline { get; set; }
        public List<ulong> SubTasks { get; set; }

        public bool IsSubTask => Parent != 0;
        public ulong Parent { get; }

        public Task(string info, DateTime deadline = default, ulong parent = 0)
        {
            Id = _nextTaskId++;
            Info = info;
            Completed = false;
            Deadline = deadline;
            SubTasks = new List<ulong>();
            Parent = parent;
        }
        
        [JsonConstructor]
        private Task(string info, ulong id, DateTime deadline, List<ulong> subTasks, ulong parent)
        {
            Id = id;
            _nextTaskId = _nextTaskId <= id ? id + 1 : _nextTaskId;
            Info = info;
            Completed = false;
            Deadline = deadline;
            SubTasks = subTasks;
            Parent = parent;
        }
    }
}