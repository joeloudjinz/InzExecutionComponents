using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Exception;

public class ExecutionPlanException : System.Exception
{
    public ExecutionPlanException(string label, System.Exception e) : base($"Execution plan [{label}] failed", e)
    {
    }

    public ExecutionPlanException(string label, IExecutionPlanContract contract, System.Exception e) : base(
        $"Execution plan [{label}] with type [{contract.ImplementationType.FullName ?? contract.ImplementationType.Name}] failed",
        e
    )
    {
    }
}