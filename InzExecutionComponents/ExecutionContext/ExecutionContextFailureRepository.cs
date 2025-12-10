using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.Models;
using InzExecutionComponents.Utilities;

namespace InzExecutionComponents.ExecutionContext;

public class ExecutionContextFailureRepository : IExecutionContextFailureRepository
{
    private readonly Dictionary<string, ExecutionFailure> _failures = [];

    public void Add(string key, ExecutionFailure failure)
    {
        _failures.Add(key, failure);
    }

    public void Fatal(string key, string code)
    {
        _failures.Add(key, ExecutionResultUtility.CodeFailure(code));
    }

    public bool HasFailures() => _failures.Count != 0;
    public bool HasFatal() => _failures.Any(f => f.Value.Fatal);
    public int Count() => _failures.Count;
}