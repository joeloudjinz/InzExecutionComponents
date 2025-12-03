using InzExecutionEvent.Attributes.Abstractions;
using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ProvideSingletonAttribute : ProviderBaseAttribute
{
    public ProvideSingletonAttribute(Type providedType, StartupModeEnum startupModeEnum) : base(providedType, startupModeEnum, ServiceScope.Singleton)
    {
    }

    public ProvideSingletonAttribute(Type providedType) : base(providedType, StartupModeEnum.Initializer | StartupModeEnum.ApiServer | StartupModeEnum.EventHandler | StartupModeEnum.DirectDrive, ServiceScope.Singleton)
    {
    }
}