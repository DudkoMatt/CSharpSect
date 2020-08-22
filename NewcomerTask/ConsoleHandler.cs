using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
                
                {"/today", Today},
                {"/add-deadline", AddDeadline},
                {"/remove-deadline", RemoveDeadline},
                
                {"/groups", Groups},
                {"/create-group", CreateGroup},
                {"/delete-group", DeleteGroup},
                {"/add-to-group", AddToGroup},
                {"/delete-from-group", DeleteFromGroup},
                
                {"/add-subtask", AddSubTask},
                
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
            Console.WriteLine("/all [group-name]");
            Console.WriteLine("/delete id");
            Console.WriteLine("/save file-name.txt");
            Console.WriteLine("/load file-name.txt");
            Console.WriteLine("/complete id");
            Console.WriteLine("/completed [group-name]");

            Console.WriteLine();
            Console.WriteLine("/today");
            Console.WriteLine("/add-deadline id DD.MM.YYYY");
            Console.WriteLine("/remove-deadline id");

            Console.WriteLine();
            Console.WriteLine("/groups");
            Console.WriteLine("/create-group group-name");
            Console.WriteLine("/delete-group group-name");
            Console.WriteLine("/add-to-group id group-name");
            Console.WriteLine("/delete-from-group id group-name");
            
            Console.WriteLine();
            Console.WriteLine("/add-subtask id subtask-info");
            
            Console.WriteLine();
            Console.WriteLine("/help");
            Console.WriteLine("/exit");
        }

        private void ReadCommand(out string command)
        {
            command = "";
            Console.Write("\n> ");
            
            _line = Console.ReadLine();
            if (_line == null) return;

            _line = _line.Trim();
            
            _args = Regex.Split(_line, @"\s+");
            command = _args[0];
        }

        private void Add()
        {
            if (_args.Length > 1)
                _taskHandler.AddNewTask(_line.Substring(_line.IndexOf(' ') + 1));
            else
                Console.WriteLine("There should be description");
        }
        
        private void All() =>
            Console.WriteLine(_args.Length == 1
                ? _taskHandler.PrintAllTasks()
                : _taskHandler.PrintAllGroup(_args[1]));

        private void Delete()
        {
            if (_args.Length == 2)
                if (ulong.TryParse(_args[1], out var res))
                    _taskHandler.DeleteTask(res);
                else
                    Console.WriteLine("Cannot convert id");
            else
                Console.WriteLine("There should be id");
        }
        
        private void Save()
        {
            if (_args.Length == 2)
                _taskHandler.SaveToFile(_args[1]);
            else
                Console.WriteLine("There should be a name of file");
        }
        
        private void Load()
        {
            if (_args.Length == 2)
                if (File.Exists(_args[1]))
                    _taskHandler.LoadFromFile(_args[1]);
                else
                    Console.WriteLine("File doesn't exists");
            else
                Console.WriteLine("There should be a name of file");
        }
        
        private void Complete()
        {
            if (_args.Length == 2)
                if (ulong.TryParse(_args[1], out var res))
                    _taskHandler.MarkCompleted(res);
                else
                    Console.WriteLine("Cannot convert id");
            else
                Console.WriteLine("There should be id");
        }
        
        private void Completed() =>
            Console.WriteLine(_args.Length == 2
                ? _taskHandler.PrintCompletedFromGroup(_args[1])
                : _taskHandler.PrintCompleted());

        private void Exit() => _running = false;
        
        private void AddDeadline()
        {
            if (_args.Length == 3)
            {
                var date = _args[2].Split('.');
                if (ulong.TryParse(_args[1], out var id) 
                    && int.TryParse(date[0], out var day)
                    && int.TryParse(date[1], out var month)
                    && int.TryParse(date[2], out var year))
                    try
                    {
                        _taskHandler.SetDeadline(id, new DateTime(year, month, day));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Date is out of range. Format: DD.MM.YYYY");
                    }
                    catch (InvalidDataException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                else
                    Console.WriteLine("Cannot parse command. See /help for syntax");
            }
            else
                Console.WriteLine("Wrong syntax. See /help for syntax");
        }

        private void RemoveDeadline()
        {
            if (_args.Length == 2)
                if (ulong.TryParse(_args[1], out var res))
                    _taskHandler.RemoveDeadline(res);
                else
                    Console.WriteLine("Cannot convert id");
            else
                Console.WriteLine("There should be id");
        }
        
        private void Today() => Console.WriteLine(_taskHandler.Today());

        private void Groups() => Console.WriteLine(_taskHandler.Groups());
        
        private void CreateGroup()
        {
            if (_args.Length == 2)
                _taskHandler.CreateGroup(_args[1]);
            else
                Console.WriteLine("There should be a name of group");
        }

        private void DeleteGroup()
        {
            if (_args.Length == 2)
                _taskHandler.DeleteGroup(_args[1]);
            else
                Console.WriteLine("There should be a name of group");
        }

        private void AddToGroup()
        {
            if (_args.Length == 3)
            {
                if (ulong.TryParse(_args[1], out var id))
                    _taskHandler.AddToGroup(id, _args[2]);
                else
                    Console.WriteLine("Cannot parse id. See /help for syntax");
            }
            else
                Console.WriteLine("Wrong syntax. See /help for syntax");
        }

        private void DeleteFromGroup()
        {
            if (_args.Length == 3)
            {
                if (ulong.TryParse(_args[1], out var id))
                    _taskHandler.DeleteFromGroup(id, _args[2]);
                else
                    Console.WriteLine("Cannot parse id. See /help for syntax");
            }
            else
                Console.WriteLine("Wrong syntax. See /help for syntax");
        }

        private void AddSubTask()
        {
            if (_args.Length > 2)
            {
                if (ulong.TryParse(_args[1], out var id))
                    _taskHandler.AddSubTask(id, _line.Substring(_line.IndexOf(' ', _line.IndexOf(' ') + 1) + 1));
                else
                    Console.WriteLine("Cannot parse id. See /help for syntax");
            }
            else
                Console.WriteLine("Wrong syntax. See /help for syntax");
        }
    }
}