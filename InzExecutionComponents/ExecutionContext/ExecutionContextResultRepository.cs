using System.Collections.Immutable;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Contracts.Models;

namespace InzExecutionComponents.ExecutionContext;

public class ExecutionContextResultRepository : ISystemExecutionContextResultRepository
{
    private readonly Dictionary<string, ExecutionResult<IExecutionResultContract>> _results = new();

    public void Add<T>(string key, ExecutionResult<T> result) where T : class, IExecutionResultContract
    {
        if (_results.ContainsKey(key)) throw new Exception($"[{key}] already exists in the result repository");
        _results.Add(key, result as ExecutionResult<IExecutionResultContract> ?? throw new Exception($"Result generic type {nameof(T)} doesn't implement {nameof(IExecutionResultContract)} interface"));
    }

    public ImmutableList<ExecutionResult<IExecutionResultContract>> Results() => _results.Values.ToImmutableList();

    public ExecutionResult<T>? Get<T>(string key) where T : class, IExecutionResultContract
    {
        if (!_results.TryGetValue(key, out var result)) return null;
        return result as ExecutionResult<T>;
    }

    public bool Any() => _results.Count != 0;

    public int Count() => _results.Count;
}