using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.Contracts.ExecutionNotification;

public interface IExecutionNotificationHandler
{
    public string HandlerName { get; set; }
    public string NotificationName { get; set; }
    public Task Handle(IServiceExecutionContext context);
}