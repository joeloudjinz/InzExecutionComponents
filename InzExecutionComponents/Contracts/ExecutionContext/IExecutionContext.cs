namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface IExecutionContext
{
    public string ExecutionPlanLabel { get; set; }
    public string ExecutionPlanInputDataKey { get; set; }
    public string ExecutionPlanRegistrationKey { get; set; }
    public IExecutionContextDataRepository MetaData { get; set; }
    public IExecutionContextDataRepository Store { get; set; }
    public IExecutionContextFailureRepository Failures { get; set; }
    public IExecutionContextResultRepository Results { get; set; }
}