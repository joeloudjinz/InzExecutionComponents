using System.Collections.Immutable;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Contracts.Models;

namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface ISystemExecutionContextResultRepository : IExecutionContextResultRepository
{
    public void Add<T>(string key, ExecutionResult<T> result) where T : class, IExecutionResultContract;
    public ImmutableList<ExecutionResult<IExecutionResultContract>> Results();
}

public interface IExecutionContextResultRepository
{
    public ExecutionResult<T>? Get<T>(string key) where T : class, IExecutionResultContract;
    public bool Any();
    public int Count();
}