namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface IExecutionContext
{
    public string ExecutionPlanLabel { get; set; }
    public string ExecutionPlanRegistrationKey { get; set; }
    public string? ExecutionPlanInputDataKey { get; set; }
    public string? ExecutionPlanOutputDataKey { get; set; }
    public IExecutionContextDataRepository Store { get; set; }
}