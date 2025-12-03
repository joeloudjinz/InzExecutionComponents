namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RequirePermissionCheckAttribute : Attribute
{
    public string[] Permissions { get; }

    public RequirePermissionCheckAttribute(string[] permissions)
    {
        Permissions = permissions;
    }

    public RequirePermissionCheckAttribute(string permission)
    {
        Permissions = [permission];
    }
}