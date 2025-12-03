using InzExecutionEvent.Attributes.Abstractions;
using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ProvideTransientAttribute : ProviderBaseAttribute
{
    public ProvideTransientAttribute(Type providedType, StartupModeEnum startupModeEnum) : base(providedType, startupModeEnum, ServiceScope.Transient)
    {
    }

    public ProvideTransientAttribute(Type providedType) : base(providedType, StartupModeEnum.Initializer | StartupModeEnum.ApiServer | StartupModeEnum.EventHandler | StartupModeEnum.DirectDrive, ServiceScope.Transient)
    {
    }
}