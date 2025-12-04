using System.Reflection;
using InzExecutionEvent.Engines;
using InzExecutionEvent.ExecutionEvent;
using InzExecutionEvent.ExecutionPlan;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionEvent;

public static class InzExecutionEvent
{
    public static IInzExecutionEventBuilder UseInzExecutionEvent(this IServiceCollection services, IConfiguration configuration)
    {
        // Register engines
        services.AddSingleton<ExecutionPlanEngine>();
        services.AddSingleton<ExecutionEventEngine>();
        services.AddSingleton<ExecutionNotificationEngine>();
        services.AddSingleton<ExecutionConfigurationEngine>();

        return new InzExecutionEventBuilder(services, configuration);
    }

    public static IInzExecutionEventBuilder RegisterExecutionComponentsFromAssembly(this IInzExecutionEventBuilder builder, Assembly assembly)
    {
        Scouters.ExecutionPlans(assembly, builder.ServiceCollection);
        Scouters.ExecutionEvents(assembly, builder.ServiceCollection);
        Scouters.ExecutionNotifications(assembly, builder.ServiceCollection);
        Scouters.ExecutionNotificationHandlers(assembly, builder.ServiceCollection);
        // TODO Discover and register execution configurations
        return builder;
    }

    public static IInzExecutionEventBuilder RegisterExecutionComponentsFromAssemblies(this IInzExecutionEventBuilder builder, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            Scouters.ExecutionPlans(assembly, builder.ServiceCollection);
            Scouters.ExecutionEvents(assembly, builder.ServiceCollection);
            Scouters.ExecutionNotifications(assembly, builder.ServiceCollection);
            Scouters.ExecutionNotificationHandlers(assembly, builder.ServiceCollection);
            // TODO Discover and register execution configurations
        }

        return builder;
    }

    public static IServiceProvider InitializeInzExecutionComponents(this IServiceProvider services)
    {
        Initializers.ExecutionPlans(services);
        Initializers.ExecutionEvents(services);
        Initializers.ExecutionNotificationsAndHandlers(services);
        // TODO Initialize execution configurations
        return services;
    }
}