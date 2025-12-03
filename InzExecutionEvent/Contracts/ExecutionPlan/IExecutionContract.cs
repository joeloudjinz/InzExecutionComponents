using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.Models;

namespace InzExecutionEvent.Contracts.ExecutionPlan;

public interface IExecutionContract<T>: IExecutionRegistryContract where T : IExecutionResultContract
{
    public Task<ExecutionResult<T>> Execute(IExecutionContext context);
}