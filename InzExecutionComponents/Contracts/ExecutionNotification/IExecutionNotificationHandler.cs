using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.Contracts.ExecutionNotification;

public interface IExecutionNotificationHandler
{
    public Task Handle(IExecutionContext context);
}