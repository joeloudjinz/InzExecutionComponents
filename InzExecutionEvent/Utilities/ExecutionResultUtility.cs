using InzExecutionEvent.Contracts.Models;
using InzExecutionEvent.Resources;

namespace InzExecutionEvent.Utilities;

public static class ExecutionResultUtility
{
    // public static ExecutionFailure<T> Success<T>(T result) where T: class, IExecutionResultContract
    // {
    //     return new ExecutionResult<T>(result);
    // }

    public static ExecutionFailure CodeFailure(string code)
    {
        return new ExecutionFailure(code);
    }

    public static ExecutionFailure ValidationFailure(string error)
    {
        return new ExecutionFailure(ExecutionFailureTypes.Validation, error);
    }

    public static ExecutionFailure FromException(Exception ex)
    {
        return new ExecutionFailure(ex);
    }
}