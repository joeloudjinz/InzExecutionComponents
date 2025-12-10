namespace InzExecutionEvent.Engines;

internal class ExecutionNotificationHandlerContract : IExecutionNotificationHandlerContract
{
    public string Name { get; set; } = string.Empty;
    public string RegistrationKey { get; set; } = string.Empty;
    public Type ImplementationType { get; set; } = null!;
    public string ImplementationTypeId { get; set; } = string.Empty;
}