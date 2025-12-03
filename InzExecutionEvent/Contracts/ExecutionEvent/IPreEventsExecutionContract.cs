using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionPlan;

namespace InzExecutionEvent.Contracts.ExecutionEvent;

public interface IPreEventsExecutionContract: IExecutionRegistryContract
{
    public Task BeforeDispatchingPreExecutionEvents(IExecutionContext context);
}