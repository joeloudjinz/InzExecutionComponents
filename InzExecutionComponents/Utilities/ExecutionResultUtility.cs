using InzExecutionComponents.Contracts.Models;
using InzExecutionComponents.Resources;

namespace InzExecutionComponents.Utilities;

public static class ExecutionResultUtility
{
    public static ExecutionFailure CodeFailure(string code)
    {
        return new ExecutionFailure(code);
    }

    public static ExecutionFailure ValidationFailure(string error)
    {
        return new ExecutionFailure(ExecutionFailureTypes.Validation, error);
    }

    public static ExecutionFailure FromException(System.Exception ex)
    {
        return new ExecutionFailure(ex);
    }
}