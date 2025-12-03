namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RegisterContextConfigOptionsAttribute : Attribute
{
    public string Label { get; }
    public Type? ContextKeysClass { get; set; }

    public RegisterContextConfigOptionsAttribute(string label)
    {
        Label = label;
    }

    public RegisterContextConfigOptionsAttribute(string label, Type contextKeysClass)
    {
        Label = label;
        ContextKeysClass = contextKeysClass;
    }
}