using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.ExecutionContext;

internal class ExecutionContext : IInternalExecutionContext
{
    public required string ExecutionPlanLabel { get; set; } = string.Empty;
    public required string ExecutionPlanRegistrationKey { get; set; } = string.Empty;
    public string? ExecutionPlanInputDataKey { get; set; }
    public string? ExecutionPlanOutputDataKey { get; set; }
    public required IExecutionContextDataRepository MetaData { get; set; } = null!;
    public required IExecutionContextDataRepository Store { get; set; } = null!;
    public required IExecutionContextFailureRepository Failures { get; set; } = null!;
}