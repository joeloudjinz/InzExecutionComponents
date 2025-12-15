using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.Models;

namespace InzExecutionComponents.Contracts.ExecutionPlan;

public interface IExecutionContract<T> : IExecutionRegistryContract where T : class, IExecutionPlanResultContract
{
    public Task<ExecutionResult<T>> Execute(IExecutionPlanContext context);
}