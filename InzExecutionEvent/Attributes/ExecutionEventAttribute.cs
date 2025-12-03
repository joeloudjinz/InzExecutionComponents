using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BaseExecutionEventAttribute : Attribute
{
    public string Name { get; protected init; } = string.Empty;
    public EventType EventType { get; protected init; }
    public string[] RequiredMetadataKeys { get; protected init; } = [];
    public string[] RequiredConfigurations { get; protected init; } = [];
    public string[] RequiredStoreKeys { get; protected init; } = [];
    public Type? InputType { get; protected init; }
    public Type? OutputType { get; protected init; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionEventAttribute : BaseExecutionEventAttribute
{
    /// <summary>
    ///     Register an execution event with the specified event type but the event doesn't require neither metadata nor
    ///     context resources.
    /// </summary>
    /// <param name="name">Event Name</param>
    /// <param name="eventType">Event Type</param>
    /// <param name="inputType">The event input type</param>
    /// <param name="outputType">The event output type</param>
    public ExecutionEventAttribute(string name, EventType eventType, Type? inputType = null, Type? outputType = null)
    {
        Name = name;
        EventType = eventType;
        RequiredMetadataKeys = [];
        RequiredStoreKeys = [];
        RequiredConfigurations = [];
        InputType = inputType;
        OutputType = outputType;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ContextExecutionEventAttribute : BaseExecutionEventAttribute
{
    /// <summary>
    ///     Register an execution event with an event type set to [EventType.Context], registered event requires the specified
    ///     metadata resources and the context resources. <br/>
    /// Use named parameters to distinguish between different constructors.
    /// </summary>
    /// <param name="name">Event Name</param>
    /// <param name="requiredMetadataKeys">Resources keys to expect in IExecutionContext.ExecutionMetaData</param>
    /// <param name="inputType">The event input type</param>
    /// <param name="outputType">The event output type</param>
    /// <param name="requiredConfigurations">Configuration options that are required to execute this event, options will be loaded into context's metadata</param>
    /// <param name="requiredStoreKeys">Resources keys to expect in IServiceExecutionContext.Store</param>
    public ContextExecutionEventAttribute(
        string name,
        string[] requiredMetadataKeys,
        Type? inputType = null,
        Type? outputType = null,
        string[]? requiredConfigurations = null,
        string[]? requiredStoreKeys = null
    )
    {
        Name = name;
        EventType = EventType.Context;
        RequiredMetadataKeys = requiredMetadataKeys;
        RequiredConfigurations = requiredConfigurations ?? [];
        RequiredStoreKeys = requiredStoreKeys ?? [];
        InputType = inputType;
        OutputType = outputType;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ServiceExecutionEventAttribute : BaseExecutionEventAttribute
{
    /// <summary>
    ///     Register an execution event with an event type set to [EventType.Service], registered event requires only the
    ///     specified context resources. <br/>
    /// Use named parameters to distinguish between different constructors.
    /// </summary>
    /// <param name="name">Event Name</param>
    /// <param name="requiredStoreKeys">Resources keys to expect in IServiceExecutionContext.Store</param>
    /// <param name="inputType">The event input type</param>
    /// <param name="outputType">The event output type</param>
    public ServiceExecutionEventAttribute(string name, string[] requiredStoreKeys, Type? inputType = null, Type? outputType = null)
    {
        Name = name;
        EventType = EventType.Service;
        RequiredStoreKeys = requiredStoreKeys;
        RequiredMetadataKeys = [];
        RequiredConfigurations = [];
        InputType = inputType;
        OutputType = outputType;
    }
}