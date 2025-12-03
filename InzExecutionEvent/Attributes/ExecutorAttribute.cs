namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutorAttribute(string group = "", string plan = "") : Attribute
{
    public string Group { get; } = group;
    public string Plan { get; } = plan;
}