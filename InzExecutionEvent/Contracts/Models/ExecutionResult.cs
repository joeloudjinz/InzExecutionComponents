using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.Contracts.Models;

public class ExecutionResult<TValue> where TValue : IExecutionResultContract
{
    private readonly TValue? _value;
    private readonly ExecutionFailure? _error;

    public TValue? Result => _value;
    public ExecutionFailure? Failure => _error;

    public bool IsError { get; }
    public bool IsSuccess => !IsError;

    private ExecutionResult(ExecutionFailure error)
    {
        IsError = false;
        _error = error;
    }

    private ExecutionResult(TValue value)
    {
        IsError = false;
        _value = value;
    }

    public static implicit operator ExecutionResult<TValue>(TValue value) => new(value);
    public static implicit operator ExecutionResult<TValue>(ExecutionFailure error) => new(error);
}