using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Contracts;

public interface IExecutionComponentManager
{
    public Task LaunchExecution(string label, IExecutionPlanParametersContract parameters);
    public Task LaunchExecution(string label);
}