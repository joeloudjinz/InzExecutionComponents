namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PublishSystemNotificationsAttribute(string[] notifications) : Attribute
{
    public string[] Notifications { get; } = notifications;
}