using System.Collections.Immutable;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.Models;

namespace InzExecutionEvent.ExecutionContext;

public class ExecutionContextResultRepository : IExecutionContextResultRepository
{
    private readonly Dictionary<string, ExecutionResult<IExecutionResultContract>> _results = new();

    public void Add<T>(ExecutionResult<T> result) where T : class, IExecutionResultContract
    {
        // if (_results.ContainsKey(result.Label)) throw new Exception($"[{result.Label}] already exists in the result repository");
        // _results.Add(result.Label, result);
    }

    public ImmutableList<ExecutionResult<IExecutionResultContract>> Results() => _results.Values.ToImmutableList();

    public bool Any() => _results.Count != 0;

    public int Count() => _results.Count;
}