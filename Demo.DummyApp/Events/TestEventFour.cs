using Demo.DummyApp.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.DummyApp.Events;

[ServiceExecutionEvent(ExecutionEventKeys.TestEvent4, [])]
public class TestEventFour : IExecutionEvent
{
    public async Task PerformEventTask(IExecutionContext context)
    {
        await Task.Delay(1500);
        Console.WriteLine($"{GetType().Name} - PerformEventTask()");
    }
}