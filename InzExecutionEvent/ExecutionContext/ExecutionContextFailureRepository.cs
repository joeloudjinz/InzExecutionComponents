using System.Collections.ObjectModel;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.Models;
using InzExecutionEvent.Utilities;

namespace InzExecutionEvent.ExecutionContext;

public class ExecutionContextFailureRepository : IExecutionContextFailureRepository
{
    private readonly List<ExecutionFailure> _failures = [];

    public void Add(ExecutionFailure failure)
    {
        _failures.Add(failure);
    }

    // public void Fatal(DataAccessFailure failure)
    // {
    //     _failures.Add(ExecutionResultUtility.FromDataAccessFailure(failure));
    // }

    public void Fatal(string code)
    {
        _failures.Add(ExecutionResultUtility.CodeFailure(code));
    }

    public bool HasFailures() => _failures.Count != 0;
    public bool HasFatal() => _failures.Any(f => f.Fatal);

    public int Count() => _failures.Count;
    public ReadOnlyCollection<ExecutionFailure> Failures() => _failures.AsReadOnly();
}