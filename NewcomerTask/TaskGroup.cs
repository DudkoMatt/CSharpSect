using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace NewcomerTask
{
    public class TaskGroup
    {
        [JsonRequired]
        private List<ulong> _taskIds;

        public TaskGroup() => _taskIds = new List<ulong>();

        public void Add(ulong taskId) => _taskIds.Add(taskId);
        public void Delete(ulong taskId) => _taskIds.Remove(taskId);
        public ReadOnlyCollection<ulong> Values() => _taskIds.AsReadOnly();
    }
}