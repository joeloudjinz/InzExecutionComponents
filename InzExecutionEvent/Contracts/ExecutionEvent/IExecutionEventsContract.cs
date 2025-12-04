using InzExecutionEvent.Contracts.ExecutionPlan;

namespace InzExecutionEvent.Contracts.ExecutionEvent;

public interface IExecutionEventsContract : IExecutionRegistryContract
{
    public string[][] PreExecutionEvents { get; set; }
    public string[][] PostExecutionEvents { get; set; }
}