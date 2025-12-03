using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionPlanAttribute(string name, string? group, ExecutionPlanType type = ExecutionPlanType.Feature) : Attribute
{
    public string Name { get; } = name;
    public string? Group { get; } = group;
    public ExecutionPlanType ExecutionPlanType { get; } = type;
}