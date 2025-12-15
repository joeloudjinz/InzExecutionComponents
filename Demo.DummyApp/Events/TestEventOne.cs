using Demo.DummyApp.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.DummyApp.Events;

[ServiceExecutionEvent(ExecutionEventKeys.TestEvent1, [])]
public class TestEventOne : IExecutionEvent
{
    public async Task Perform(IExecutionEventContext context)
    {
        await Task.Delay(1500);
        Console.WriteLine($"{GetType().Name} - PerformEventTask()");
    }
}