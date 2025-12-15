using InzExecutionComponents.Enums;

namespace InzExecutionComponents.ExecutionEvent;

internal interface IExecutionEventContract
{
    public string RegistrationKey { get; set; }
    public Type ImplementationType { get; set; }
    public string Name { get; set; }
    public EventType Type { get; set; }
    public string[] RequiredMetadataKeys { get; set; }
    public string[] RequiredConfigurations { get; set; }
    public string[] RequiredStoreKeys { get; set; }
    public string[] RequiredSystemNotifications { get; set; }
}