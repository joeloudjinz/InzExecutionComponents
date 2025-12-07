namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterPostExecutionEvents(params string[] events): Attribute
{
    public string[] Events { get; set; } = events;
}