using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionPlan;

namespace InzExecutionEvent.Contracts.ExecutionEvent;

public interface IPostEventsExecutionContract : IExecutionRegistryContract
{
    public Task AfterDispatchingPostExecutionEvents(IExecutionContext context);
}