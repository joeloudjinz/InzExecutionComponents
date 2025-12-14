using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface IExecutionPlanContext
{
    public string ExecutionPlanRegistrationKey { get; }
    public IExecutionContextDataRepository Store { get; }

    public T GetInputData<T>() where T : class, IExecutionPlanParametersContract;
    public T GetOutputData<T>() where T : class, IExecutionPlanResultContract;
}