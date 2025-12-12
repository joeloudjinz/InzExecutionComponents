namespace InzExecutionComponents.Attributes;

// Should be used on a class that implements either IExecutionParametersContract or IExecutionResultContract
[AttributeUsage(AttributeTargets.Property)]
public class ExecutionContextStoreKeyAttribute(string key) : Attribute
{
    public string Key { get; set; } = key;
}