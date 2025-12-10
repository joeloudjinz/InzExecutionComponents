using System.Reflection;
using InzExecutionEvent.Contracts;
using InzExecutionEvent.Engines;
using InzExecutionEvent.ExecutionEvent;
using InzExecutionEvent.ExecutionPlan;
using InzExecutionEvent.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionEvent;

public static class InzExecutionComponents
{
    public static IServiceCollection AddInzExecutionComponents(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        var executionNotificationEngine = new ExecutionNotificationEngine();
        var executionConfigurationEngine = new ExecutionConfigurationEngine();
        var executionEventEngine = new ExecutionEventEngine(executionConfigurationEngine, executionNotificationEngine);
        var executionPlanEngine = new ExecutionPlanEngine(executionEventEngine, executionNotificationEngine, executionConfigurationEngine);

        // Register engines
        services.AddSingleton(executionPlanEngine);
        services.AddSingleton(executionEventEngine);
        services.AddSingleton(executionNotificationEngine);
        services.AddSingleton(executionConfigurationEngine);
        services.AddSingleton<IExecutionComponentManager, ExecutionComponentManager>();

        foreach (var assembly in assemblies)
        {
            executionPlanEngine.RegisterExecutionPlans(assembly, services);

            Scouters.ExecutionEvents(assembly, services);
            executionEventEngine.RegisterEvents(Scouters.ExecutionEventTypes(assembly));

            executionNotificationEngine.RegisterExecutionNotificationAndHandlers(assembly, services);

            // TODO enable execution configuration feature
            // Scouters.ExecutionNotificationHandlers(assembly, services);
            // Scouters.ExecutionConfigurations(assembly, configuration);
        }

        return services;
    }

    public static IServiceProvider UseInzExecutionComponents(this IServiceProvider services)
    {
        var executionPlanEngine = services.GetRequiredService<ExecutionPlanEngine>();
        executionPlanEngine.StartEngine(services);

        var executionNotificationEngine = services.GetRequiredService<ExecutionNotificationEngine>();
        executionNotificationEngine.StartEngine(services);

        return services;
    }
}