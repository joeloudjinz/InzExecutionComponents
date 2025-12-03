namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RequireConfigurationOptionsAttribute(string[] labels) : Attribute
{
    public string[] Labels { get; } = labels;
}