using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Contracts.Models;

public class ExecutionResult<TValue> where TValue : class, IExecutionPlanResultContract
{
    public TValue? Result { get; }
    public ExecutionFailure? Failure { get; }
    public bool IsError { get; }
    public bool IsSuccess => !IsError;

    private ExecutionResult(ExecutionFailure error)
    {
        IsError = true;
        Failure = error;
    }

    private ExecutionResult(TValue value)
    {
        IsError = false;
        Result = value;
    }

    public static implicit operator ExecutionResult<TValue>(TValue value) => new(value);
    public static implicit operator ExecutionResult<TValue>(ExecutionFailure error) => new(error);
}