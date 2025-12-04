using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Contracts.ExecutionNotification;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Engines;
using InzExecutionEvent.ExecutionEvent;
using InzExecutionEvent.ExecutionPlan;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionEvent.Utilities;

public static class Initializers
{
    public static void ExecutionPlans(IServiceProvider services)
    {
        var executionPlanEngine = services.GetRequiredService<ExecutionPlanEngine>();
        var plans = services.GetServices<IExecutionRegistryContract>().ToList();
        if (plans.Count == 0)
        {
            Console.Error.WriteLine("No execution plans found!"); // TODO improve the error message
            return;
        }

        executionPlanEngine.RegisterExecutionPlans(plans);
    }

    public static void ExecutionEvents(IServiceProvider services)
    {
        var executionEventEngine = services.GetRequiredService<ExecutionEventEngine>();
        var events = services.GetServices<IExecutionEvent>().ToList();
        if (events.Count == 0) 
        {
            Console.Error.WriteLine("No execution events found!"); // TODO improve the error message
            return;
        }
        
        executionEventEngine.RegisterEvents(events);
    } 
    
    public static void ExecutionNotificationsAndHandlers(IServiceProvider services)
    {
        var executionNotificationEngine = services.GetRequiredService<ExecutionNotificationEngine>();
        var notifications = services.GetServices<IExecutionNotification>().ToList();
        if (notifications.Count == 0)
        {
            Console.Error.WriteLine("No execution notifications found!"); // TODO improve the error message
            return;
        }

        var handlers = services.GetServices<IExecutionNotificationHandler>().ToList();
        if (handlers.Count == 0)
        {
            Console.Error.WriteLine("No handlers found for execution notifications!"); // TODO improve the error message
            return;
        }

        executionNotificationEngine.RegisterNotificationAndHandlers(notifications, handlers);
    }
}