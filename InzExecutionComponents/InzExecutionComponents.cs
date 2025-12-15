using System.Reflection;
using InzExecutionComponents.Contracts;
using InzExecutionComponents.ExecutionEvent;
using InzExecutionComponents.ExecutionNotification;
using InzExecutionComponents.ExecutionPlan;
using InzExecutionComponents.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionComponents;

public static class InzExecutionComponents
{
    public static IServiceCollection AddInzExecutionComponents(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        var executionNotificationEngine = new ExecutionNotificationEngine();
        var executionEventEngine = new ExecutionEventEngine(executionNotificationEngine);
        var executionPlanEngine = new ExecutionPlanEngine(executionEventEngine, executionNotificationEngine);

        // Register engines
        services.AddSingleton(executionPlanEngine);
        services.AddSingleton(executionEventEngine);
        services.AddSingleton(executionNotificationEngine);
        services.AddSingleton<IExecutionComponentManager, ExecutionComponentManager>();

        const string executionTimeRecorderKey = "Registering execution components from assemblies";
        ExecutionTimeRecorder.Start(executionTimeRecorderKey);

        foreach (var assembly in assemblies)
        {
            executionPlanEngine.RegisterExecutionPlans(assembly, services);
            executionEventEngine.RegisterExecutionEvents(assembly, services);
            executionNotificationEngine.RegisterExecutionNotificationAndHandlers(assembly, services);
        }

        ExecutionTimeRecorder.EndThenPrint(executionTimeRecorderKey);
        return services;
    }

    public static IServiceProvider UseInzExecutionComponents(this IServiceProvider services)
    {
        var executionPlanEngine = services.GetRequiredService<ExecutionPlanEngine>();
        executionPlanEngine.StartEngine(services);

        var executionEventEngine = services.GetRequiredService<ExecutionEventEngine>();
        executionEventEngine.StartEngine(services);

        var executionNotificationEngine = services.GetRequiredService<ExecutionNotificationEngine>();
        executionNotificationEngine.StartEngine(services);

        return services;
    }
}