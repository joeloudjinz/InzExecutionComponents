using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.Contracts.ExecutionEvent;

public interface IExecutionEvent
{
    public Task PerformEventTask(IExecutionContext context);
}