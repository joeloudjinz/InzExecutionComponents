namespace InzExecutionComponents.ExecutionNotification;

internal class ExecutionNotificationContract : IExecutionNotificationContract
{
    public string NotificationName { get; set; } = string.Empty;
    public IExecutionNotificationHandlerContract[] Handlers { get; set; } = [];
}