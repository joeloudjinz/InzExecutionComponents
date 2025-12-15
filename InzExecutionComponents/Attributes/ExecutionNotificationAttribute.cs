namespace InzExecutionComponents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionNotificationAttribute(string name, string[] requiredStoreKeys, string[] providedStoreKeys) : Attribute
{
    public string Name { get; set; } = name;
    public string[] RequiredStoreKeys { get; set; } = requiredStoreKeys;
    public string[] ProvidedStoreKeys { get; set; } = providedStoreKeys;

    public ExecutionNotificationAttribute(string name) : this(name, [], [])
    {
    }

    public ExecutionNotificationAttribute(string name, string[] requiredStoreKeys) : this(name, requiredStoreKeys, [])
    {
    }
}