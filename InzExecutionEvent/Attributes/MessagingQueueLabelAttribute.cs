namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MessagingQueueLabelAttribute(string label) : Attribute
{
    public string Label { get; } = label;
}