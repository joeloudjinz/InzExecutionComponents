namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ExecutionConfigurationOptionsAttribute(string[] labels) : Attribute
{
    public string[] Labels { get; } = labels;
}