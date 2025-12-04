using Demo.DummyApp.Resources;
using InzExecutionEvent.Contracts.ExecutionNotification;

namespace Demo.DummyApp.ExecutionNotifications;

public class TestExecutionNotification : IExecutionNotification
{
    public string Name { get; set; } = ExecutionNotificationKeys.Test;
    public string[] RequiredStoreKeys { get; set; } = [];
    public string[] ProvidedStoreKeys { get; set; } = [];
}