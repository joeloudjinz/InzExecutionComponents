namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionNotificationHandlerAttribute(string notificationName, string handlerName) : Attribute
{
    public string HandlerName { get; set; } = handlerName;
    public string NotificationName { get; set; } = notificationName;
}