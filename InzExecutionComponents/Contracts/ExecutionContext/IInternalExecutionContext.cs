namespace InzExecutionComponents.Contracts.ExecutionContext;

internal interface IInternalExecutionContext : IExecutionContext
{
    public IExecutionContextDataRepository MetaData { get; set; }
    public IExecutionContextFailureRepository Failures { get; set; }
}