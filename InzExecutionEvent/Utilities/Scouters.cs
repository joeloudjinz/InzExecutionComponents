using System.Diagnostics;
using System.Reflection;
using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionNotification;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionEvent.Utilities;

internal static class Scouters
{
    public static Type[] ExecutionPlanTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => type.GetCustomAttributes().Any(a => a is ExecutionPlanAttribute)).ToArray();
    }

    public static void ExecutionPlans(Assembly assembly, IServiceCollection collection)
    {
        var stopwatch = Stopwatch.StartNew();
        var map = ExecutionPlanTypes(assembly).ToDictionary(
            type => type,
            type => type.GetCustomAttributes().Where(a => a is ExecutionPlanAttribute).ToList()
        );

        foreach (var (type, attributes) in map)
        foreach (var attribute in attributes)
        {
            if (attribute is not ExecutionPlanAttribute _) continue;
            collection.AddSingleton(typeof(IExecutionRegistryContract), type);
        }

        stopwatch.Stop();
        Console.WriteLine($"{assembly.GetName().Name} => Execution plans discovery and registration took [{stopwatch.ElapsedMilliseconds} ms].");
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
        return assembly.GetTypes().Where(type => type.GetInterfaces().Any(i => i == typeof(IExecutionNotification))).ToArray();
    }

    public static void ExecutionNotifications(Assembly assembly, IServiceCollection collection)
    {
        var stopwatch = Stopwatch.StartNew();
        var list = ExecutionNotificationTypes(assembly);
        foreach (var type in list) collection.AddSingleton(typeof(IExecutionNotification), type);
        stopwatch.Stop();
        Console.WriteLine($"{assembly.GetName().Name} => Execution notifications discovery and registration took [{stopwatch.ElapsedMilliseconds} ms].");
    }

    public static void ExecutionNotificationHandlers(Assembly assembly, IServiceCollection collection)
    {
        var stopwatch = Stopwatch.StartNew();
        var list = assembly.GetTypes().Where(type => type.GetInterfaces().Any(i => i == typeof(IExecutionNotificationHandler))).ToList();
        foreach (var type in list) collection.AddSingleton(typeof(IExecutionNotificationHandler), type);
        stopwatch.Stop();
        Console.WriteLine($"{assembly.GetName().Name} => Execution notification handlers discovery and registration took [{stopwatch.ElapsedMilliseconds} ms].");
    }

    public static void ExecutionConfigurations(Assembly assembly, IConfiguration configuration)
    {
        var stopwatch = Stopwatch.StartNew();
        // TODO implement this method
        stopwatch.Stop();
        Console.WriteLine($"{assembly.GetName().Name} => Execution configurations discovery and registration took [{stopwatch.ElapsedMilliseconds} ms].");
    }
}