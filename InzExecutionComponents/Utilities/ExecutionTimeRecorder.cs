using System.Diagnostics;

namespace InzExecutionComponents.Utilities;

public static class ExecutionTimeRecorder
{
    private static readonly Dictionary<string, Stopwatch> Stopwatches = new();

    public static void Start(string key) => Stopwatches.TryAdd(key, Stopwatch.StartNew());

    public static void EndThenPrint(string key)
    {
        End(key);
        Print(key);
    }

    public static void End(string key)
    {
        if (Stopwatches.TryGetValue(key, out var value))
        {
            value.Stop();
            return;
        }

        Console.Error.WriteLine($"Stopwatch with key [{key}] was not found");
    }

    public static void Print(string key)
    {
        if (Stopwatches.Remove(key, out var value))
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[ExecutionTimeRecorder]: [{key}] => {value.ElapsedMilliseconds}ms");
            Console.ResetColor();
            Console.WriteLine();
            return;
        }

        Console.Error.WriteLine($"Stopwatch with key [{key}] was not found");
    }
}