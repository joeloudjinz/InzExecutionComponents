namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PublishExecutionNotificationsAttribute(string[] notifications) : Attribute
{
    public string[] Notifications { get; } = notifications;
}