using InzExecutionComponents.Enums;

namespace InzExecutionComponents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BaseExecutionEventAttribute : Attribute
{
    public string Name { get; protected init; } = string.Empty;
    public EventType EventType { get; protected init; }
    public string[] RequiredMetadataKeys { get; protected init; } = [];
    public string[] RequiredConfigurations { get; protected init; } = [];
    public string[] RequiredStoreKeys { get; protected init; } = [];
}

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionEventAttribute : BaseExecutionEventAttribute
{
    public ExecutionEventAttribute(string name, EventType eventType)
    {
        Name = name;
        EventType = eventType;
        RequiredMetadataKeys = [];
        RequiredStoreKeys = [];
        RequiredConfigurations = [];
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ServiceExecutionEventAttribute : BaseExecutionEventAttribute
{
    public ServiceExecutionEventAttribute(string name)
    {
        Name = name;
        EventType = EventType.Service;
    }

    public ServiceExecutionEventAttribute(
        string name,
        string[] requiredStoreKeys
    )
    {
        Name = name;
        EventType = EventType.Service;
        RequiredStoreKeys = requiredStoreKeys;
    }

    public ServiceExecutionEventAttribute(
        string name,
        string[] requiredStoreKeys,
        string[] requiredMetadataKeys
    )
    {
        Name = name;
        EventType = EventType.Service;
        RequiredStoreKeys = requiredStoreKeys;
        RequiredMetadataKeys = requiredMetadataKeys;
    }

    public ServiceExecutionEventAttribute(
        string name,
        string[] requiredStoreKeys,
        string[]? requiredMetadataKeys = null,
        string[]? requiredConfigurations = null
    )
    {
        Name = name;
        EventType = EventType.Service;
        RequiredStoreKeys = requiredStoreKeys;
        RequiredMetadataKeys = requiredMetadataKeys ?? [];
        RequiredConfigurations = requiredConfigurations ?? [];
    }
}