namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RegisterExecutionConfigOptionsAttribute : Attribute
{
    public string Label { get; }
    public Type? ContextKeysClass { get; set; }

    public RegisterExecutionConfigOptionsAttribute(string label)
    {
        Label = label;
    }

    public RegisterExecutionConfigOptionsAttribute(string label, Type contextKeysClass)
    {
        Label = label;
        ContextKeysClass = contextKeysClass;
    }
}