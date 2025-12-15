using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Attributes;

public class ExecutionInputDataTypeAttribute<T>() : BaseExecutionPlanDataTypeAttribute(typeof(T), isInput: true) where T : IExecutionPlanParametersContract;
public class ExecutionOutputDataTypeAttribute<T>() : BaseExecutionPlanDataTypeAttribute(typeof(T), isInput: false) where T : IExecutionPlanResultContract;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseExecutionPlanDataTypeAttribute(Type type, bool isInput) : Attribute
{
    public Type Type { get; } = type;
    public bool IsInput { get; } = isInput;
}