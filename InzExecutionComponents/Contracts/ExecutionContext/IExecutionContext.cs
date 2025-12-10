namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface IExecutionContext
{
    public IExecutionContextDataRepository MetaData { get; set; }
    public IExecutionContextDataRepository Store { get; set; }
    public IExecutionContextFailureRepository Failures { get; set; }
    public IExecutionContextResultRepository Results { get; set; }
}