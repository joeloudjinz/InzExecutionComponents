using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface IExecutionPlanContext : IExecutionEventContext
{
    public string ExecutionPlanRegistrationKey { get; }
    public T GetInputData<T>() where T : class, IExecutionPlanParametersContract;
    public T GetOutputData<T>() where T : class, IExecutionPlanResultContract;
}