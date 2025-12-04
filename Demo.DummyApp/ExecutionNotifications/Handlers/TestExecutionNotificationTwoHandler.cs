using Demo.DummyApp.Resources;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionNotification;

namespace Demo.DummyApp.ExecutionNotifications.Handlers;

public class TestExecutionNotificationTwoHandler : IExecutionNotificationHandler
{
    public string HandlerName { get; set; } = ExecutionNotificationHandlerKeys.Test.Test2;
    public string NotificationName { get; set; } = ExecutionNotificationKeys.Test;

    public async Task Handle(IServiceExecutionContext context)
    {
        await Task.Delay(1500);
        Console.WriteLine($"{HandlerName}: {NotificationName}");
    }
}