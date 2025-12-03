namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ResponseDataTypeAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;
}