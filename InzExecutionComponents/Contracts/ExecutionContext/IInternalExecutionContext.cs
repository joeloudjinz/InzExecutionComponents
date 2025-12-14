using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Contracts.ExecutionContext;

internal interface IInternalExecutionContext : IExecutionPlanContext
{
    public IExecutionContextDataRepository MetaData { get; }
    public IExecutionContextFailureRepository Failures { get; }
    
    public void SetInputData<T>(T data) where T : class, IExecutionPlanParametersContract;
    public void SetOutputData<T>(T data) where T : class, IExecutionPlanResultContract;
}