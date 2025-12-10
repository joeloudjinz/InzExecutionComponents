using Demo.DummyApp.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionNotification;

namespace Demo.DummyApp.ExecutionNotifications.Handlers;

[ExecutionNotificationHandler(ExecutionNotificationKeys.Test, ExecutionNotificationHandlerKeys.Test.Test1)]
public class TestExecutionNotificationOneHandler : IExecutionNotificationHandler
{
    public async Task Handle(IExecutionContext context)
    {
        await Task.Delay(1500);
        Console.WriteLine($"{GetType().Name} executed");
    }
}