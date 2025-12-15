using InzExecutionComponents.Enums;

namespace InzExecutionComponents.ExecutionEvent;

internal class EventContract : IExecutionEventContract
{
    public string RegistrationKey { get; set; } = string.Empty;
    public Type ImplementationType { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public string[] RequiredMetadataKeys { get; set; } = [];
    public string[] RequiredConfigurations { get; set; } = [];
    public string[] RequiredStoreKeys { get; set; } = [];
    public string[] RequiredSystemNotifications { get; set; } = [];
}