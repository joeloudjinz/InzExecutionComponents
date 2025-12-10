using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.ExecutionContext;

public class CoreExecutionContext : IExecutionContext
{
    public IExecutionContextDataRepository MetaData { get; set; }
    public IExecutionContextDataRepository Store { get; set; }
    public IExecutionContextFailureRepository Failures { get; set; }
    public IExecutionContextResultRepository Results { get; set; }
}