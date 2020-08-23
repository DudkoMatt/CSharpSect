namespace NewcomerTask
{
    class Program
    {
        public static void Main(string[] args)
        {
            var taskHandler = new TaskHandler();
            new ConsoleHandler(taskHandler).Run();
        }
    }
}