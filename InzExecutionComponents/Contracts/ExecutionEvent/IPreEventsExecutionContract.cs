using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Contracts.ExecutionEvent;

public interface IPreEventsExecutionContract : IExecutionRegistryContract
{
    public Task BeforeDispatchingPreExecutionEvents(IExecutionPlanContext context);
}