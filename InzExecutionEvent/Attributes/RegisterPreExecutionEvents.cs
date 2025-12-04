namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterPreExecutionEvents(params string[][] events) : Attribute
{
    public string[][] Events { get; set; } = events;
}