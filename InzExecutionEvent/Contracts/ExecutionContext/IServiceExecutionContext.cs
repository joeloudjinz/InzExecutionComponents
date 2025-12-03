namespace InzExecutionEvent.Contracts.ExecutionContext;

public interface IServiceExecutionContext
{
    public IExecutionContextDataRepository Store { get; set; }
    public IExecutionContextFailureRepository Failures { get; set; }
}