namespace InzExecutionComponents.ExecutionNotification;

internal interface IExecutionNotificationContract
{
    public string NotificationName { get; set; }
    public IExecutionNotificationHandlerContract[] Handlers { get; set; }
}