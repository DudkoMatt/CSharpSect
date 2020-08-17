using System;
using System.Collections.Generic;
using System.IO;

namespace NewcomerTask
{
    public class ConsoleHandler
    {
        private string _line;
        private string[] _args;
        private bool _running;
        private readonly TaskHandler _taskHandler;
        private readonly Dictionary<string, Action> _dictionary;
        
        public ConsoleHandler(TaskHandler taskHandler)
        {
            _running = true;
            _taskHandler = taskHandler;
            _dictionary = new Dictionary<string, Action>()
            {
                {"/add", Add},
                {"/all", All},
                {"/delete", Delete},
                {"/save", Save},
                {"/load", Load},
                {"/complete", Complete},
                {"/completed", Completed},
                {"/help", Help},
                {"/exit", Exit}
            };
        }

        public void Run()
        {
            Help();
            while (_running)
            {
                ReadCommand(out var command);

                if (_dictionary.ContainsKey(command))
                    _dictionary[command]();
                else
                    Console.WriteLine("Wrong command. Type /help for help");
            }
        }

        private static void Help()
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

        private void ReadCommand(out string command)
        {
            command = "";
            Console.Write("\n> ");
            
            _line = Console.ReadLine();
            if (_line == null) return;
            
            _args = _line.Split();
            command = _args[0];
        }

        private void Add()
        {
            if (_args.Length > 1)
                _taskHandler.AddNewTask(_line.Substring(_line.IndexOf(' ') + 1));
            else
                Console.WriteLine("There should be description");
        }
        
        private void All() => Console.WriteLine(_taskHandler.PrintAllTasks());

        private void Delete()
        {
            if (_args.Length > 1)
                if (ulong.TryParse(_args[1], out var res))
                    _taskHandler.DeleteTask(res);
                else
                    Console.WriteLine("Cannot convert id");
            else
                Console.WriteLine("There should be id");
        }
        
        private void Save()
        {
            if (_args.Length > 1)
                _taskHandler.SaveToFile(_line.Substring(_line.IndexOf(' ')));
            else
                Console.WriteLine("There should be a name of file");
        }
        
        private void Load()
        {
            if (_args.Length > 1)
                if (File.Exists(_line.Substring(_line.IndexOf(' '))))
                    _taskHandler.LoadFromFile(_line.Substring(_line.IndexOf(' ')));
                else
                    Console.WriteLine("File doesn't exists");
            else
                Console.WriteLine("There should be a name of file");
        }
        
        private void Complete()
        {
            if (_args.Length > 1)
                if (ulong.TryParse(_args[1], out var res))
                    _taskHandler.MarkCompleted(res);
                else
                    Console.WriteLine("Cannot convert id");
            else
                Console.WriteLine("There should be id");
        }
        
        private void Completed() => Console.WriteLine(_taskHandler.PrintCompleted());

        private void Exit() => _running = false;
    }
}