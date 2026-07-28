// *Day 3 — Async/Await Deep Dive & Concurrency Basics 

// *3.1 The Task-Based Asynchronous Pattern
/*class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Start");

        await DoWorkAsync();

        Console.WriteLine("End");

    }
    static async Task DoWorkAsync()
    {
        Console.WriteLine("Working...");

        await Task.Delay(3000); // Wait 3 seconds without blocking the thread

        Console.WriteLine("Finished!");
    }
}*/
// * 3.2 Async All the Way
/*class Program
{
    static void Main()
    {
        Console.WriteLine("Start");

        string data = GetDataAsync().Result; // Blocks the thread

        Console.WriteLine(data);
        Console.WriteLine("End");
    }

    static async Task<string> GetDataAsync()
    {
        await Task.Delay(3000);
        return "Hello";
    }
    
}*/
// *3.4 Cancellation Tokens
/*
using System;
// !with out Task.WhenAll
class Program
{
    static async Task Main()
    {
        await Task1();
        await Task2();
        await Task3();

        Console.WriteLine("Done");
    }

    static async Task Task1()
    {
        await Task.Delay(2000);
        Console.WriteLine("Task 1 Finished");
    }

    static async Task Task2()
    {
        await Task.Delay(2000);
        Console.WriteLine("Task 2 Finished");
    }

    static async Task Task3()
    {
        await Task.Delay(2000);
        Console.WriteLine("Task 3 Finished");
    }
}


class Program
{
 user  Task.WhenAll
    static async Task Main()
    {
        Task t1 = Task1();
        Task t2 = Task2();
        Task t3 = Task3();

        await Task.WhenAll(t1, t2, t3);

        Console.WriteLine("Done");
    }

    static async Task Task1()
    {
        await Task.Delay(2000);
        Console.WriteLine("Task 1 Finished");
    }

    static async Task Task2()
    {
        await Task.Delay(2000);
        Console.WriteLine("Task 2 Finished");
    }

    static async Task Task3()
    {
        await Task.Delay(2000);
        Console.WriteLine("Task 3 Finished");
    }
}*/
//* Hands-On Lab: Concurrent Async Operations
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("===== Sequential =====");

        Stopwatch sw = Stopwatch.StartNew();

        await DatabaseAsync();
        await ApiAsync();
        await FileAsync();

        sw.Stop();
        Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");

        Console.WriteLine("\n===== Task.WhenAll =====");

        sw.Restart();

        Task db = DatabaseAsync();
        Task api = ApiAsync();
        Task file = FileAsync();

        await Task.WhenAll(db, api, file);

        sw.Stop();
        Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");

        Console.WriteLine("\n===== Cancellation =====");

        CancellationTokenSource cts = new();

        Task cancelTask = LongOperationAsync(cts.Token);

        cts.CancelAfter(2000);

        try
        {
            await cancelTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation Cancelled!");
        }
    }

    static async Task DatabaseAsync()
    {
        await Task.Delay(3000);
        Console.WriteLine("Database Finished");
    }

    static async Task ApiAsync()
    {
        await Task.Delay(3000);
        Console.WriteLine("API Finished");
    }

    static async Task FileAsync()
    {
        await Task.Delay(3000);
        Console.WriteLine("File Finished");
    }

    static async Task LongOperationAsync(CancellationToken token)
    {
        Console.WriteLine("Long Operation Started...");

        await Task.Delay(5000, token);

        Console.WriteLine("Long Operation Finished");
    }
}