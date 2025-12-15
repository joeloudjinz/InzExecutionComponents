namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface IExecutionEventContext
{
    public IExecutionContextDataRepository Store { get; }
}