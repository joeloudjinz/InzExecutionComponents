using System.Collections.ObjectModel;
using InzExecutionEvent.Contracts.Models;

namespace InzExecutionEvent.Contracts.ExecutionContext;

public interface IExecutionContextFailureRepository
{
    public void Add(ExecutionFailure failure);
    public void Fatal(string code);
    public ReadOnlyCollection<ExecutionFailure> Failures();
    public bool HasFailures();
    public bool HasFatal();
    public int Count();
}