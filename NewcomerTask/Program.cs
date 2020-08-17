namespace NewcomerTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var taskHandler = new TaskHandler();
            ConsoleHandler.Run(taskHandler);
        }
    }
}