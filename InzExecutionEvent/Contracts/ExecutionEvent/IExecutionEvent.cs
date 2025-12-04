using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.Contracts.ExecutionEvent;

public interface IExecutionEvent : IBaseEvent
{
    public Task PerformEventTask(IServiceExecutionContext context);
}