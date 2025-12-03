
using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.Contracts.ExecutionEvent;

public interface IContextExecutionEvent : IBaseEvent
{
    public Task UpdateContext(IExecutionContext context);
}