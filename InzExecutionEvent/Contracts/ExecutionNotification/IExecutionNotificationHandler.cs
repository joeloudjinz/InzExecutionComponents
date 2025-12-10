using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.Contracts.ExecutionNotification;

public interface IExecutionNotificationHandler
{
    public Task Handle(IServiceExecutionContext context);
}