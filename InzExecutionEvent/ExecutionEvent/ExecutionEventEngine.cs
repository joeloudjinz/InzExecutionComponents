using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Engines;

namespace InzExecutionEvent.ExecutionEvent;

internal class ExecutionEventEngine(ExecutionConfigurationEngine configurationEngine, ExecutionNotificationEngine executionNotificationEngine)
{
    private static readonly Type[] ProcessableAttributes =
    [
        typeof(BaseExecutionEventAttribute),
        typeof(ExecutionEventAttribute),
        typeof(PublishExecutionNotificationsAttribute),
        typeof(ServiceExecutionEventAttribute)
    ];

    private readonly Dictionary<string, EventContract> _registeredEventsContracts = new();

    public void RegisterEvents(ICollection<Type> events)
    {
        foreach (var type in events)
        {
            var attributes = type.GetCustomAttributes(true).Where(a => ProcessableAttributes.Contains(a.GetType())).ToList();
            var eventName = BuildAndRegisterEventContract(type, attributes);
            ProcessEventAttributesAndUpdateEventContract(eventName, attributes);
        }
    }

    private void ProcessEventAttributesAndUpdateEventContract(string eventName, List<object> attributes)
    {
        foreach (var attribute in attributes)
        {
            if (attribute is not PublishExecutionNotificationsAttribute systemNotifications) continue;
            if (systemNotifications.Notifications.Length == 0) continue;
            _registeredEventsContracts[eventName].RequiredSystemNotifications = systemNotifications.Notifications;
            // Register more data for the event by processing other attribute here ...
        }
    }

    private string BuildAndRegisterEventContract(Type eventType, List<object> attributes)
    {
        if (attributes.FirstOrDefault(a => a is BaseExecutionEventAttribute) is not BaseExecutionEventAttribute executionEventData)
        {
            throw new Exception("Null object of [BaseExecutionEventAttribute] attribute!");
        }

        var model = new EventContract
        {
            InstanceType = eventType,
            Name = executionEventData.Name,
            Type = executionEventData.EventType,
            RequiredMetadataKeys = executionEventData.RequiredMetadataKeys,
            RequiredConfigurations = executionEventData.RequiredConfigurations,
            RequiredStoreKeys = executionEventData.RequiredStoreKeys,
            InputType = executionEventData.InputType,
            OutputType = executionEventData.OutputType
        };

        _registeredEventsContracts.Add(executionEventData.Name, model);
        return model.Name;
    }

    public async Task DispatchEvents(IExecutionContext context, Queue<string[]> map)
    {
        foreach (var events in map)
        {
            await Task.WhenAll(events.Select(e => ProcessEventTypeAndDispatchEvent(context, e)));
            if (!context.Failures.HasFatal()) continue;
            break;
        }
    }

    public async Task DispatchEvents(IExecutionContext context, string[][] map)
    {
        foreach (var events in map)
        {
            await Task.WhenAll(events.Select(e => ProcessEventTypeAndDispatchEvent(context, e)));
            if (!context.Failures.HasFatal()) continue;
            break;
        }
    }

    private async Task ProcessEventTypeAndDispatchEvent(IExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var contract)) throw new Exception($"Event [{name}] not found");

        CheckIfEventRequiresContextMetadataResources(context, name);
        CheckIfEventRequiresContextStoreResources(context, name);
        CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(context, name);

        var instance = EventEngineUtilities.GetExecutionEventInstanceAndCastToEventInterface<IExecutionEvent>(context.ServiceProvider, contract);
        await instance.PerformEventTask(context);

        // TODO enable execution notification feature
        // await CheckIfEventPublishesSystemNotificationAndPublish(context, name);
    }

    // private Task CheckIfEventPublishesSystemNotificationAndPublish(IExecutionContext context, string name)
    // {
    //     if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new Exception($"Event [{name}] not found");
    //     return model.RequiredSystemNotifications.Length == 0
    //         ? Task.CompletedTask
    //         : executionNotificationEngine.HandleNotifications(context, model.RequiredSystemNotifications);
    // }

    private void CheckIfEventRequiresContextMetadataResources(IExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new Exception($"Event [{name}] not found");
        if (model.RequiredMetadataKeys.Length == 0) return;
        context.MetaData.Check(model.RequiredMetadataKeys);
    }

    private void CheckIfEventRequiresContextStoreResources(IServiceExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new Exception($"Event [{name}] not found");
        if (model.RequiredStoreKeys.Length == 0) return;
        context.Store.Check(model.RequiredStoreKeys);
    }

    private void CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(IExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new Exception($"Event [{name}] not found");
        if (model.RequiredConfigurations.Length == 0) return;
        configurationEngine.LoadConfigurationOptionsIntoContext(context, model.RequiredConfigurations);
    }
}