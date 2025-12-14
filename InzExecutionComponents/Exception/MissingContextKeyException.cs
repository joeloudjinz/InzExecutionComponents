using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.ExecutionEvent;

namespace InzExecutionComponents.Exception;

internal class MissingContextKeyException : System.Exception
{
    public MissingContextKeyException(string storageType, string key, EventContract contract) : base(
        $"Required context {storageType} key [{key}] is missing in the context {storageType} for execution event [{contract.Name}] with implementation type [{contract.InstanceType.FullName ?? contract.InstanceType.Name}]"
    )
    {
    }

    public MissingContextKeyException(string storageType, string key, IExecutionPlanContract contract) : base(
        $"Required context {storageType} key [{key}] is missing in the context {storageType} for execution plane [{contract.Label}] with implementation type [{contract.ImplementationType.FullName ?? contract.ImplementationType.Name}]"
    )
    {
    }
    
    public MissingContextKeyException(string key, IExecutionPlanContract contract) : base(
        $"Context key [{key}] is missing for execution plane [{contract.Label}] with implementation type [{contract.ImplementationType.FullName ?? contract.ImplementationType.Name}]"
    )
    {
    }
}