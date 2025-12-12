using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Engines;
using InzExecutionComponents.Exception;

namespace InzExecutionComponents.ExecutionEvent;

internal class ExecutionEventEngine(ExecutionConfigurationEngine configurationEngine, ExecutionNotificationEngine executionNotificationEngine)
{
    private IServiceProvider ServiceProvider { get; set; } = null!;

    private static readonly Type[] ProcessableAttributes =
    [
        typeof(BaseExecutionEventAttribute),
        typeof(ExecutionEventAttribute),
        typeof(PublishExecutionNotificationsAttribute),
        typeof(ServiceExecutionEventAttribute)
    ];

    private readonly Dictionary<string, EventContract> _registeredEventsContracts = new();

    public void StartEngine(IServiceProvider services)
    {
        ServiceProvider = services;
    }

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
            throw new System.Exception("Null object of [BaseExecutionEventAttribute] attribute!");
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

    public async Task DispatchEvents(IInternalExecutionContext context, Queue<string[]> map)
    {
        foreach (var events in map)
        {
            await Task.WhenAll(events.Select(e => ProcessEventTypeAndDispatchEvent(context, e)));
            if (!context.Failures.HasFatal()) continue;
            break;
        }
    }

    public async Task DispatchEvents(IInternalExecutionContext context, string[][] map)
    {
        foreach (var events in map)
        {
            await Task.WhenAll(events.Select(e => ProcessEventTypeAndDispatchEvent(context, e)));
            if (!context.Failures.HasFatal()) continue;
            break;
        }
    }

    private async Task ProcessEventTypeAndDispatchEvent(IInternalExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var contract)) throw new InvalidOperationException($"Execution event [{name}] was not found");

        CheckIfEventRequiresContextMetadataResources(context, name, contract);
        CheckIfEventRequiresContextStoreResources(context, name, contract);
        // CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(context, name);

        var instance = EventEngineUtilities.GetExecutionEventInstanceAndCastToEventInterface<IExecutionEvent>(ServiceProvider, contract);
        try
        {
            await instance.PerformEventTask(context);
        }
        catch (System.Exception e)
        {
            throw new ExecutionEventException(name, contract, e);
        }

        // TODO enable execution notification feature
        // await CheckIfEventPublishesSystemNotificationAndPublish(context, name);
    }

    // private Task CheckIfEventPublishesSystemNotificationAndPublish(IInternalExecutionContext context, string name)
    // {
    //     if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new InvalidOperationException($"Execution event [{was name}] not found");
    //     return model.RequiredSystemNotifications.Length == 0
    //         ? Task.CompletedTask
    //         : executionNotificationEngine.HandleNotifications(context, model.RequiredSystemNotifications);
    // }

    private void CheckIfEventRequiresContextMetadataResources(IInternalExecutionContext context, string name, EventContract contract)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new InvalidOperationException($"Execution event [{name}] was not found");
        if (model.RequiredMetadataKeys.Length == 0) return;

        var results = context.MetaData.Check(model.RequiredMetadataKeys);
        if (results.success) return;

        throw new MissingContextKeyException(storageType: "metadata", key: results.missing, contract);
    }

    private void CheckIfEventRequiresContextStoreResources(IInternalExecutionContext context, string name, EventContract contract)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new InvalidOperationException($"Execution event [{name}] was not found");
        if (model.RequiredStoreKeys.Length == 0) return;

        var results = context.Store.Check(model.RequiredStoreKeys);
        if (results.success) return;

        throw new MissingContextKeyException(storageType: "store", key: results.missing, contract);
    }

    // private void CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(IInternalExecutionContext context, string name)
    // {
    //     if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new InvalidOperationException($"Execution event [{name}] was not found");
    //     if (model.RequiredConfigurations.Length == 0) return;
    //     configurationEngine.LoadConfigurationOptionsIntoContext(context, model.RequiredConfigurations);
    // }
}