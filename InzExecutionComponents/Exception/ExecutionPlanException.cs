using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Exception;

internal class ExecutionPlanException : System.Exception
{
    public ExecutionPlanException(string label, System.Exception e) : base($"Execution plan [{label}] failed, see inner exception for details", e)
    {
    }

    public ExecutionPlanException(IExecutionPlanContract contract, System.Exception e) : base(
        $"Execution plan [{contract.Label}] with type [{contract.ImplementationType.FullName ?? contract.ImplementationType.Name}] failed, see inner exception for details",
        e
    )
    {
    }
}