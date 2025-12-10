using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.Models;

namespace InzExecutionComponents.Contracts.ExecutionPlan;

public interface IExecutionContract<T> : IExecutionRegistryContract where T : IExecutionResultContract
{
    public Task<ExecutionResult<T>> Execute(IExecutionContext context);
}