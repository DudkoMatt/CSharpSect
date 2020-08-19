using System;
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

        public Task(string info, DateTime deadline = default)
        {
            Id = _nextTaskId++;
            Info = info;
            Completed = false;
            Deadline = deadline;
        }
        
        [JsonConstructor]
        private Task(string info, ulong id, DateTime deadline)
        {
            Id = id;
            _nextTaskId = _nextTaskId <= id ? id + 1 : _nextTaskId;
            Info = info;
            Completed = false;
            Deadline = deadline;
        }
    }
}