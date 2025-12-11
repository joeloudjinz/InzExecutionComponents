using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.ExecutionContext;

public class CoreExecutionContext : IExecutionContext
{
    public string ExecutionPlanLabel { get; set; } = string.Empty;
    public string ExecutionPlanInputDataKey { get; set; } = string.Empty;
    public string ExecutionPlanRegistrationKey { get; set; } = string.Empty;
    public IExecutionContextDataRepository MetaData { get; set; }
    public IExecutionContextDataRepository Store { get; set; }
    public IExecutionContextFailureRepository Failures { get; set; }
    public IExecutionContextResultRepository Results { get; set; }
}