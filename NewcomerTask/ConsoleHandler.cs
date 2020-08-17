using System;
using System.IO;

namespace NewcomerTask
{
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
        public static void Run(TaskHandler taskHandler)
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
                        Console.WriteLine(taskHandler.PrintAllTasks());
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
                                taskHandler.LoadFromFile(line.Substring(line.IndexOf(' ')));
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
                        Console.WriteLine(taskHandler.PrintCompleted());
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
}