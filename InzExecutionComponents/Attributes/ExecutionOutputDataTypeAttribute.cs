namespace InzExecutionComponents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionOutputDataTypeAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;
}