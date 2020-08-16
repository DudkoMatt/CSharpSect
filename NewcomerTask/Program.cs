using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        [JsonRequired]
        private List<Task> _tasks;

        public TaskHandler()
        {
            _tasks = new List<Task>();
        }
        
        public void AddNewTask(string info) => _tasks.Add(new Task(info));
        
        public Task GetAt(int idx) => _tasks[idx];
        
        [JsonIgnore]
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
                f.Write(JsonConvert.SerializeObject(this));
            }
        }

        public static TaskHandler LoadFromFile(string filename)
        {
            using (var f = File.OpenText(filename))
            {
                return JsonConvert.DeserializeObject<TaskHandler>(f.ReadToEnd());
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

    public static class ConsoleHandler
    {
        public static void Hello()
        {
            Console.WriteLine("TaskHandler");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine("/add task-info");
            Console.WriteLine("/all");
            Console.WriteLine("/delete id");
            Console.WriteLine("/save file-name.txt");
            Console.WriteLine("/load file-name.txt");
            Console.WriteLine("/complete id");
            Console.WriteLine("/completed");
            Console.WriteLine("/help");
            Console.WriteLine("/exit");
        }
        public static void Run(ref TaskHandler taskHandler)
        {
            Hello();
            var running = true;
            while (running)
            {
                Console.Write("\n> ");
                var line = Console.ReadLine();
                if (line == null) return;
                var args = line.Split();
                var command = args[0];

                switch (command)
                {
                    case "/add":
                        if (args.Length > 1)
                            taskHandler.AddNewTask(line.Substring(line.IndexOf(' ') + 1));
                        else
                            Console.WriteLine("There should be description");
                        break;
                    case "/all":
                        taskHandler.PrintAllTasks();
                        break;
                    case "/delete":
                        if (args.Length > 1)
                            if (ulong.TryParse(args[1], out var res))
                                taskHandler.DeleteTask(res);
                            else
                                Console.WriteLine("Cannot convert id");
                        else
                            Console.WriteLine("There should be id");
                        break;
                    case "/save":
                        if (args.Length > 1)
                            taskHandler.SaveToFile(line.Substring(line.IndexOf(' ')));
                        else
                            Console.WriteLine("There should be a name of file");
                        break;
                    case "/load":
                        if (args.Length > 1)
                            if (File.Exists(line.Substring(line.IndexOf(' '))))
                                taskHandler = TaskHandler.LoadFromFile(line.Substring(line.IndexOf(' ')));
                            else
                                Console.WriteLine("File doesn't exists");
                        else
                            Console.WriteLine("There should be a name of file");
                        break;
                    case "/complete":
                        if (args.Length > 1)
                            if (ulong.TryParse(args[1], out var res))
                                taskHandler.MarkCompleted(res);
                            else
                                Console.WriteLine("Cannot convert id");
                        else
                            Console.WriteLine("There should be id");
                        break;
                    case "/completed":
                        taskHandler.PrintCompleted();
                        break;
                    case "/exit":
                        running = false;
                        break;
                    case "/help":
                        Hello();
                        break;
                    default:
                        Console.WriteLine("Wrong command. Type /help for help");
                        break;
                }
            }
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var taskHandler = new TaskHandler();
            ConsoleHandler.Run(ref taskHandler);
        }
    }
}