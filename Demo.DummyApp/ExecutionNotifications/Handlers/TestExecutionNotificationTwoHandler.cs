using Demo.DummyApp.Resources;
using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionNotification;

namespace Demo.DummyApp.ExecutionNotifications.Handlers;

[ExecutionNotificationHandler(ExecutionNotificationKeys.Test, ExecutionNotificationHandlerKeys.Test.Test2)]
public class TestExecutionNotificationTwoHandler : IExecutionNotificationHandler
{
    public async Task Handle(IExecutionContext context)
    {
        await Task.Delay(1500);
        Console.WriteLine($"{GetType().Name} executed");
    }
}