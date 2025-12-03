using System.Collections.Immutable;
using InzExecutionEvent.Contracts.Models;

namespace InzExecutionEvent.Contracts.ExecutionContext;

public interface IExecutionContextResultRepository
{
    public void Add<T>(ExecutionResult<T> result) where T : class, IExecutionResultContract;
    public ImmutableList<ExecutionResult<IExecutionResultContract>> Results();
    public bool Any();
    public int Count();
}