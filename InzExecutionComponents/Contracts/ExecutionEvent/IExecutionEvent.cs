using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.Contracts.ExecutionEvent;

public interface IExecutionEvent
{
    public Task PerformEventTask(IExecutionPlanContext context);
}