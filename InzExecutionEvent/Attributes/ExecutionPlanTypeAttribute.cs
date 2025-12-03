using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionPlanTypeAttribute(ExecutionPlanType type) : Attribute
{
    public ExecutionPlanType ExecutionPlanType { get; set; } = type;
}