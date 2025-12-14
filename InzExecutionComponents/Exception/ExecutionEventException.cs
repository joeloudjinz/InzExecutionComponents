using InzExecutionComponents.ExecutionEvent;

namespace InzExecutionComponents.Exception;

internal class ExecutionEventException : System.Exception
{
    public ExecutionEventException(string label, System.Exception e) : base($"Execution event [{label}] failed", e)
    {
    }

    public ExecutionEventException(string label, EventContract contract, System.Exception e) : base(
        $"Execution event [{label}] of type [{contract.InstanceType.FullName ?? contract.InstanceType.Name}] failed",
        e
    )
    {
    }
}