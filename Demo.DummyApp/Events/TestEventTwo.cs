using Demo.DummyApp.Resources;
using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionEvent;

namespace Demo.DummyApp.Events;

[ServiceExecutionEvent(ExecutionEventKeys.TestEvent2, [])]
public class TestEventTwo: IExecutionEvent
{
    public async Task PerformEventTask(IExecutionContext context)
    {
        await Task.Delay(1500);
        Console.WriteLine($"{GetType().Name} - PerformEventTask()");
    }
}