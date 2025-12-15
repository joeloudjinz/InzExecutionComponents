using Demo.DummyApp.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionNotification;

namespace Demo.DummyApp.ExecutionNotifications.Handlers;

[ExecutionNotificationHandler(ExecutionNotificationKeys.TestEvents, ExecutionNotificationHandlerKeys.Test.Test3)]
public class TestEventsExecutionNotificationThreeHandler: IExecutionNotificationHandler
{
    public async Task Handle(IExecutionEventContext context)
    {
        await Task.Delay(1500);
        Console.WriteLine($"{GetType().Name} executed");
    }
}