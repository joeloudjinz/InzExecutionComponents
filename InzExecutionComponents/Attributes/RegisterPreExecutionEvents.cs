namespace InzExecutionComponents.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterPreExecutionEvents(params string[] events) : Attribute
{
    public string[] Events { get; set; } = events;
}