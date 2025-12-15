using System.Diagnostics;
using System.Reflection;
using InzExecutionComponents.Attributes;
using Microsoft.Extensions.Configuration;

namespace InzExecutionComponents.Utilities;

internal static class Scouters
{
    public static Type[] ExecutionPlanTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => type.GetCustomAttributes().Any(a => a is ExecutionPlanAttribute)).ToArray();
    }

    public static Type[] ExecutionEventTypes(Assembly assembly)
    {
        return assembly.DefinedTypes.Where(type => type.GetCustomAttributes().Any(i => i is BaseExecutionEventAttribute)).ToArray<Type>();
    }

    public static Type[] ExecutionNotificationTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => type.GetCustomAttributes().Any(a => a is ExecutionNotificationAttribute)).ToArray();
    }

    public static Type[] ExecutionNotificationHandlerTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => type.GetCustomAttributes().Any(a => a is ExecutionNotificationHandlerAttribute)).ToArray();
    }

    public static void ExecutionConfigurations(Assembly assembly, IConfiguration configuration)
    {
        var stopwatch = Stopwatch.StartNew();
        // TODO implement this method
        stopwatch.Stop();
        Console.WriteLine($"{assembly.GetName().Name} => Execution configurations discovery and registration took [{stopwatch.ElapsedMilliseconds} ms].");
    }
}