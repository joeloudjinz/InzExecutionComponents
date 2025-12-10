using InzExecutionComponents.Contracts.Models;

namespace InzExecutionComponents.Contracts.ExecutionContext;

public interface IExecutionContextFailureRepository
{
    public void Add(string key, ExecutionFailure failure);
    public void Fatal(string key, string code);
    public bool HasFailures();
    public bool HasFatal();
    public int Count();
}