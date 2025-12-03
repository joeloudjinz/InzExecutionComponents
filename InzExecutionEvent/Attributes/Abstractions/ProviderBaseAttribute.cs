using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes.Abstractions;

public class ProviderBaseAttribute(Type providedType, StartupModeEnum startupModeEnum, ServiceScope scope) : Attribute
{
    public Type ProvidesType { get; } = providedType;
    public StartupModeEnum StartupModeEnum { get; } = startupModeEnum;
    public ServiceScope Scope { get; } = scope;
}