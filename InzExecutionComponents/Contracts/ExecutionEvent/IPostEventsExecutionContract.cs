using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Contracts.ExecutionEvent;

public interface IPostEventsExecutionContract : IExecutionRegistryContract
{
    public Task AfterDispatchingPostExecutionEvents(IExecutionContext context);
}