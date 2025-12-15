using InzExecutionComponents.Enums;

namespace InzExecutionComponents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionPlanAttribute : Attribute
{
    public ExecutionPlanAttribute(string name)
    {
        Name = name;
    }

    public ExecutionPlanAttribute(string name, string group)
    {
        Name = name;
        Group = group;
    }

    public ExecutionPlanAttribute(string name, string? group, ExecutionPlanType type = ExecutionPlanType.Feature)
    {
        Name = name;
        Group = group;
        ExecutionPlanType = type;
    }

    public string Name { get; }
    public string? Group { get; }
    public ExecutionPlanType ExecutionPlanType { get; } = ExecutionPlanType.Feature;
}