using InzExecutionComponents.Enums;

namespace InzExecutionComponents.ExecutionEvent;

public class EventContract
{
    public Type InstanceType { get; init; }
    public string Name { get; init; } = string.Empty;
    public EventType Type { get; init; }
    public string[] RequiredMetadataKeys { get; init; } = [];
    public string[] RequiredConfigurations { get; init; } = [];
    public string[] RequiredStoreKeys { get; init; } = [];
    public string[] RequiredSystemNotifications { get; set; } = [];
    public Type? InputType { get; set; }
    public Type? OutputType { get; set; }
}