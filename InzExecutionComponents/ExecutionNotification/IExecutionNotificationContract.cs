namespace InzExecutionComponents.ExecutionNotification;

internal interface IExecutionNotificationContract
{
    public IExecutionNotificationHandlerContract[] Handlers { get; set; }
}