using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.Contracts.ExecutionEvent;

public interface IExecutionEvent
{
    public Task Perform(IExecutionEventContext context);
}