using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionComponents.ExecutionEvent;

public static class EventEngineUtilities
{
    public static T GetExecutionEventInstanceAndCastToEventInterface<T>(IServiceProvider provider, EventContract model)
    {
        var instances = provider.GetServices(model.InstanceType).ToList();
        if (instances is { Count: 0 }) throw new System.Exception($"Event [{model.Name}] implementation is not found in DI container.");
        if (instances is { Count: > 1 }) throw new System.Exception($"Event [{model.Name}] has multiple registered implementations that implements [{typeof(T).Name}].");

        var instance = instances.FirstOrDefault();
        if (instance is null) throw new System.Exception($"Event [{model.Name}] instance is null!");
        return (T)instance ?? throw new System.Exception($"Event [{model.Name}] has a type [{model.Type.ToString()}] mismatch with its instance");
    }
}