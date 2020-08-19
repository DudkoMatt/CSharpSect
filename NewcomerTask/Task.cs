using Newtonsoft.Json;

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
        
        [JsonConstructor]
        private Task(string info, ulong id, DateTime deadline)
        {
            Id = id;
            _nextTaskId = _nextTaskId <= id ? id + 1 : _nextTaskId;
            Info = info;
            Completed = false;
        }
    }
}