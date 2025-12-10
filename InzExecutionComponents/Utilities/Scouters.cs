using System.Diagnostics;
using System.Reflection;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    public static void ExecutionEvents(Assembly assembly, IServiceCollection collection)
    {
        var stopwatch = Stopwatch.StartNew();
        var map = ExecutionEventTypes(assembly).ToDictionary(
            type => type,
            type => type.GetCustomAttributes().Where(i => i is BaseExecutionEventAttribute).ToList()
        );

        foreach (var (type, attributes) in map)
        foreach (var attribute in attributes)
        {
            if (attribute is not BaseExecutionEventAttribute baseExecutionEventAttribute) continue;
            if (baseExecutionEventAttribute.EventType == EventType.Service)
            {
                collection.AddSingleton(type);
                // collection.AddSingleton(typeof(IExecutionEvent), type);
                continue;
            }

            Console.Error.WriteLine($"{type.Name} has event type of {baseExecutionEventAttribute.EventType}, the event is not registered");
        }

        stopwatch.Stop();
        Console.WriteLine($"{assembly.GetName().Name} => Events discovery and registration took [{stopwatch.ElapsedMilliseconds} ms].");
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