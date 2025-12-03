using System.Collections.Immutable;
using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Enums;
using InzExecutionEvent.Utilities;

namespace InzExecutionEvent.Engines;

[ProvideSingleton(typeof(EventEngine))]
public class EventEngine(ExecutionConfigurationEngine configurationEngine, ExecutionNotificationEngine executionNotificationEngine)
{
    public static readonly Type[] EventTypesToRegister =
    [
        typeof(IContextExecutionEvent),
        typeof(IExecutionEvent),
        // typeof(IDataAccessEvent<IDataAccessEventResult>),
        // typeof(IDataAccessEvent<IDataAccessEventParameters, IDataAccessEventResult>)
    ];

    public static readonly Type[] ProcessableAttributes =
    [
        typeof(PublishSystemNotificationsAttribute),
        typeof(BaseExecutionEventAttribute),
        typeof(ExecutionEventAttribute),
        typeof(ServiceExecutionEventAttribute),
        typeof(ContextExecutionEventAttribute),
        typeof(DataAccessExecutionEventAttribute)
    ];

    private readonly Dictionary<string, EventContract> _registeredEventsContracts = new();

    public ImmutableDictionary<string, EventContract> RegisteredEventsContracts() => _registeredEventsContracts.ToImmutableDictionary();

    // public void RegisterEvents(IEnumerable<IBaseEvent> events)
    // {
    //     foreach (var e in events)
    //     {
    //         var attributes = e.GetType().GetCustomAttributes(true).Where(a => ProcessableAttributes.Contains(a.GetType())).ToList();
    //         var eventName = BuildAndRegisterEventContract(e, attributes);
    //         ProcessEventAttributesAndUpdateEventContract(eventName, attributes);
    //     }
    // }

    public void RegisterEvents(IEnumerable<Type> events)
    {
        foreach (var e in events)
        {
            var attributes = e.GetCustomAttributes(true).Where(a => ProcessableAttributes.Contains(a.GetType())).ToList();
            var eventName = BuildAndRegisterEventContract(e, attributes);
            ProcessEventAttributesAndUpdateEventContract(eventName, attributes);
        }
    }

    private void ProcessEventAttributesAndUpdateEventContract(string eventName, List<object> attributes)
    {
        foreach (var attribute in attributes)
        {
            if (attribute is not PublishSystemNotificationsAttribute systemNotifications) continue;
            if (!systemNotifications.Notifications.Any()) continue;
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

        // model.Instance = executionEventData.EventType switch
        // {
        //     EventType.Service => Activator.CreateInstance(eventType) as IExecutionEvent ?? throw new NullReferenceException($"{executionEventData.Name} is not an IExecutionEvent"),
        //     EventType.Context => Activator.CreateInstance(eventType) as IContextExecutionEvent ?? throw new NullReferenceException($"{executionEventData.Name} is not an IContextExecutionEvent"),
        //     EventType.DataAccess => InstantiateDataAccessEvent(eventType, executionEventData) ?? throw new NullReferenceException($"{executionEventData.Name} is not an IDataAccessEvent"),
        //     _ => throw new ArgumentOutOfRangeException($"{executionEventData.EventType.ToString()}")
        // };

        _registeredEventsContracts.Add(executionEventData.Name, model);
        return model.Name;
    }

    // private IBaseEvent? InstantiateDataAccessEvent(Type eventType, BaseExecutionEventAttribute executionEventData)
    // {
        // if (executionEventData.InputType is null && executionEventData.OutputType is not null) eventType = eventType.MakeGenericType(executionEventData.OutputType);
        // else if (executionEventData.InputType is not null && executionEventData.OutputType is not null) eventType = eventType.MakeGenericType(executionEventData.InputType, executionEventData.OutputType);
        // else throw new Exception($"{executionEventData.Name} doesn't have valid data access event signature, double check that input and output types are defined according to the implemented interface.");
        // return Activator.CreateInstance(eventType) as IBaseEvent;
    // }

    // private string BuildAndRegisterEventContract(IBaseEvent instance, List<object> attributes)
    // {
    //     if (attributes.FirstOrDefault(a => a is BaseExecutionEventAttribute) is not BaseExecutionEventAttribute executionEventData)
    //     {
    //         throw new Exception("Null object of [BaseExecutionEventAttribute] attribute!");
    //     }
    //
    //     var model = new EventContract
    //     {
    //         // Instance = instance,
    //         Name = executionEventData.Name,
    //         Type = executionEventData.EventType,
    //         RequiredMetadataKeys = executionEventData.RequiredMetadataKeys,
    //         RequiredConfigurations = executionEventData.RequiredConfigurations,
    //         RequiredStoreKeys = executionEventData.RequiredStoreKeys,
    //         InputType = executionEventData.InputType,
    //         OutputType = executionEventData.OutputType
    //     };
    //     _registeredEventsContracts.Add(executionEventData.Name, model);
    //     return model.Name;
    // }

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

    private Task ProcessEventTypeAndDispatchEvent(IExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new Exception($"Event [{name}] not found");
        switch (model.Type)
        {
            case EventType.Context:
            {
                var instance = EventEngineUtilities.GetExecutionEventInstanceAndCastToEventInterface<IContextExecutionEvent>(context.ServiceProvider, model);
                // if (model.Instance is not IContextExecutionEvent instance) throw new Exception($"Event [{name}] has a type [{model.Type.ToString()}] mismatch with its instance");
                CheckIfEventRequiresMetadataResourcesContext(context, name);
                CheckIfEventRequiresStoreResourcesAndLoadResourcesIntoContext(context, name);
                CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(context, name);
                instance.UpdateContext(context);
                return CheckIfEventPublishesSystemNotificationAndPublish(context, name);
            }
            case EventType.Service:
            {
                var instance = EventEngineUtilities.GetExecutionEventInstanceAndCastToEventInterface<IExecutionEvent>(context.ServiceProvider, model);
                // if (model.Instance is not IExecutionEvent instance) throw new Exception($"Event [{name}] has a type [{model.Type.ToString()}] mismatch with its instance");
                CheckIfEventRequiresStoreResourcesAndLoadResourcesIntoContext(context, name);
                instance.PerformEventTask(context);
                return CheckIfEventPublishesSystemNotificationAndPublish(context, name);
            }
            case EventType.DataAccess:
                return Task.CompletedTask;
            // case EventType.DataAccess:
            // {
            //     if (model.Instance is not IDataAccessEvent serviceEvent) throw new Exception($"Event [{name}] has a type [{model.Type.ToString()}] mismatch with its instance");
            //     CheckIfEventRequiresStoreResourcesAndLoadResourcesIntoContext(context, name);
            //     CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(context, name);
            //     // Suppose to call the service method here, but couldn't cast the instance to IDataAccessEvent because of the generic arguments 
            //     return CheckIfEventPublishesSystemNotificationAndPublish(context, name);
            // }
            default:
                throw new Exception($"Event [{name}] not a sub-type of contract IBaseEvent");
        }
    }

    private Task CheckIfEventPublishesSystemNotificationAndPublish(IExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new Exception($"Event [{name}] not found");
        if (model.RequiredSystemNotifications.Length == 0) return Task.CompletedTask;
        return executionNotificationEngine.HandleNotifications(context, model.RequiredSystemNotifications);
    }

    private void CheckIfEventRequiresMetadataResourcesContext(IExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var model)) throw new Exception($"Event [{name}] not found");
        if (model.RequiredMetadataKeys.Length == 0) return;
        context.MetaData.Check(model.RequiredMetadataKeys);
    }

    private void CheckIfEventRequiresStoreResourcesAndLoadResourcesIntoContext(IServiceExecutionContext context, string name)
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

// TODO: move into a separate file
public class EventContract
{
    // public IBaseEvent Instance { get; set; }
    public Type InstanceType { get; set; }
    public string Name { get; set; }
    public EventType Type { get; set; }
    public string[] RequiredMetadataKeys { get; set; } = [];
    public string[] RequiredConfigurations { get; set; } = [];
    public string[] RequiredStoreKeys { get; set; } = [];
    public string[] RequiredSystemNotifications { get; set; } = [];
    public Type? InputType { get; set; }
    public Type? OutputType { get; set; }
}