namespace NewcomerTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var taskHandler = new TaskHandler();
            new ConsoleHandler(taskHandler).Run();
        }
    }
}