using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var service = new ItemIntegrationService();
        
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        //To check the parallel insertion of elements, we can queue
        // some more elements and wait for 2.1 seconds, as each
        // element needs 2 seconds, everything should be added after
        // that time if it's being inserted in parallel.

        
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("x"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("y"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("z"));
        Console.WriteLine("Everything before 2.1 seconds:");
        service.GetAllItems().ForEach(Console.WriteLine);
        Thread.Sleep(2_100);
        Console.WriteLine("Everything after 2.1 seconds:");
        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}